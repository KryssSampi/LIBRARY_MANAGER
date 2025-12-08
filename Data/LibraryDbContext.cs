using LIBBRARY_MANAGER.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LIBBRARY_MANAGER.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Subscriber> Subscribers { get; set; } = null!;
        public DbSet<StaffMember> StaffMembers { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<Modification> Modifications { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public LibraryDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source=./Data/Library_Manager.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== Configuration User ==========
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                // ✅ CORRECTION: Laisser EF générer les IDs automatiquement
                entity.Property(e => e.Id_User)
                    .ValueGeneratedOnAdd(); // Au lieu de ValueGeneratedNever()
                entity.HasIndex(e => e.Adresse_Mail).IsUnique();
                entity.Property(e => e.Date_Creation).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ========== Configuration Subscriber ==========
            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.ToTable("Subscribers");
                entity.HasIndex(e => e.Ref_Subscriber).IsUnique();
                entity.Property(e => e.Fidelity).HasPrecision(4, 2);

                entity.HasMany(s => s.Loans)
                    .WithOne(l => l.Subscriber)
                    .HasForeignKey(l => l.SubscriberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== Configuration StaffMember ==========
            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.ToTable("StaffMembers");
                entity.HasIndex(e => e.Ref_Staff).IsUnique();

                entity.HasMany(s => s.Modifications)
                    .WithOne(m => m.StaffMember_)
                    .HasForeignKey(m => m.StaffMemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== Configuration Book ==========
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasIndex(e => e.ISBN).IsUnique();
                entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Quantity).IsConcurrencyToken();
            });

            // ========== Configuration Loan ==========
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loans");
                entity.HasIndex(e => e.Ref_Loan).IsUnique();
                entity.Property(e => e.BorrowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Penalty).HasPrecision(10, 2);

                entity.HasOne(l => l.Book)
                    .WithMany()
                    .HasForeignKey(l => l.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Subscriber)
                    .WithMany(s => s.Loans)
                    .HasForeignKey(l => l.SubscriberId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.Modifications)
                    .WithOne(m => m.Loan_)
                    .HasForeignKey(m => m.LoanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== Configuration Modification ==========
            modelBuilder.Entity<Modification>(entity =>
            {
                entity.ToTable("Modifications");
                entity.HasIndex(e => e.Ref_Modification).IsUnique();
                entity.Property(e => e.ModificationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(m => m.StaffMember_)
                    .WithMany(s => s.Modifications)
                    .HasForeignKey(m => m.StaffMemberId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Loan_)
                    .WithMany(l => l.Modifications)
                    .HasForeignKey(m => m.LoanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== Configuration Event ==========
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");
                entity.Property(e => e.EventId).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

        // ✅ MÉTHODE AJOUTÉE pour StaffMember.cs
        public void GenerateReferences()
        {
            GenerateReferencesForNewEntities();
            if (ChangeTracker.HasChanges())
            {
                base.SaveChanges();
            }
        }

        /// <summary>
        /// Sauvegarde avec génération automatique des références
        /// </summary>
        public void SaveChangesWithReferences()
        {
            // 1. Premier SaveChanges pour obtenir les IDs auto-générés
            base.SaveChanges();

            // 2. Générer les références pour les nouvelles entités
            GenerateReferencesForNewEntities();

            // 3. Deuxième SaveChanges pour persister les références
            if (ChangeTracker.HasChanges())
            {
                base.SaveChanges();
            }
        }

        /// <summary>
        /// Version async
        /// </summary>
        public async Task SaveChangesWithReferencesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            GenerateReferencesForNewEntities();

            if (ChangeTracker.HasChanges())
            {
                await base.SaveChangesAsync(cancellationToken);
            }
        }

        private void GenerateReferencesForNewEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Unchanged)
                .ToList();

            foreach (var entry in entries)
            {
                switch (entry.Entity)
                {
                    case Loan loan when string.IsNullOrEmpty(loan.Ref_Loan) && loan.LoanId > 0:
                        loan.GenerateReference();
                        entry.State = EntityState.Modified;
                        break;

                    case Subscriber subscriber when string.IsNullOrEmpty(subscriber.Ref_Subscriber) && subscriber.Id_User > 0:
                        subscriber.GenerateReference();
                        entry.State = EntityState.Modified;
                        break;

                    case StaffMember staff when string.IsNullOrEmpty(staff.Ref_Staff) && staff.Id_User > 0:
                        staff.GenerateReference();
                        entry.State = EntityState.Modified;
                        break;

                    case Modification modification when string.IsNullOrEmpty(modification.Ref_Modification) && modification.ModificationId > 0:
                        modification.GenerateReference();
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }

       
    }
}