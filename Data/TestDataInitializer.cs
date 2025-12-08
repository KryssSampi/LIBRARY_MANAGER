using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LIBBRARY_MANAGER.Model;
using static System.Reflection.Metadata.BlobBuilder;

namespace LIBBRARY_MANAGER.Data
{
    public static class TestDataInitializer
    {
        /// <summary>
        /// Initialise toutes les données de test si la base est vide
        /// </summary>
        public static async Task InitializeTestDataAsync(LibraryDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //Optionnel: Vérification de la base
            if (await context.Books.AnyAsync() || await context.Subscribers.AnyAsync() || await context.StaffMembers.AnyAsync())
            {
                Console.WriteLine("La base contient déjà des données. Initialisation ignorée.");
                return;
            }
            try
            {
                Console.WriteLine("Initialisation des données de test...");

                await CreateTestStaffMembersAsync(context);
                await CreateTestSubscribersAsync(context);
                await CreateTestBooksAsync(context);
                await CreateTestLoansAsync(context);
                await CreateTestEventsAsync(context);

                Console.WriteLine("✅ Données de test initialisées avec succès!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("⚠️ ERREUR SQLITE : " + ex.InnerException?.Message);
                Console.WriteLine("Pile d'appel : " + ex);
                throw new Exception("⚠️ ERREUR SQLITE : " + ex.InnerException?.Message);
            }
        }

        // =====================================
        // MÉTHODES ASYNCHRONES
        // =====================================

        private static async Task CreateTestEventsAsync(LibraryDbContext context)
        {
            if (await context.Events.AnyAsync())
            {
                Console.WriteLine("La table Events contient déjà des données. Ignoré.");
                return;
            }

            var events = new[]
            {
        new Event
        {
            Title = "Soirée Jeux de Société",
            Description = "Un événement pour découvrir de nouveaux jeux et rencontrer d'autres passionnés.",
            StartDate = DateTime.Now.AddDays(2),
            EndDate = DateTime.Now.AddDays(2).AddHours(3)
        },
        new Event
        {
            Title = "Atelier Lecture Jeunesse",
            Description = "Atelier destiné aux enfants de 5 à 10 ans.",
            StartDate = DateTime.Now.AddDays(5).AddHours(10),
            EndDate = DateTime.Now.AddDays(5).AddHours(12)
        },
        new Event
        {
            Title = "Conférence d'un Auteur Invité",
            Description = "L'auteur célèbre Jean Tremblay viendra parler de son dernier roman.",
            StartDate = DateTime.Now.AddDays(8).AddHours(18),
            EndDate = DateTime.Now.AddDays(8).AddHours(20)
        },
        new Event
        {
            Title = "Café-Lecture du Vendredi",
            Description = "Discussion sur le livre du mois.",
            StartDate = DateTime.Now.AddDays(12).AddHours(17),
            EndDate = DateTime.Now.AddDays(12).AddHours(19)
        }
    };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

            Console.WriteLine($"✓ {events.Length} événements créés");
        }

        private static async Task CreateTestStaffMembersAsync(LibraryDbContext context)
        {
            var staffMembers = new[]
            {
                new StaffMember
                {
                    Name_User = "Admin Principal",
                    Adresse_Mail = "admin@library.ca",
                    Num_Telephone = "613-555-0001",
                    Adresse = "123 Rue Wellington, Ottawa, ON K1A 0A9",
                    Poste = "Directeur",
                    YearHired = 2015,
                    Password = "admin123",
                     BirthDate = new DateTime(new Random().Next((DateTime.Now.Year-100),(DateTime.Now.Year-12)),new Random().Next(1, 12), new Random().Next(1, 28))
                },
                new StaffMember
                {
                    Name_User = "Marie Bibliothecaire",
                    Adresse_Mail = "marie@library.ca",
                    Num_Telephone = "613-555-0002",
                    Adresse = "456 Avenue Bank, Ottawa, ON K1P 5L1",
                    Poste = "Bibliothécaire",
                    YearHired = 2018,
                    Password = "marie123",
                BirthDate = new DateTime(new Random().Next((DateTime.Now.Year-100),(DateTime.Now.Year-12)),new Random().Next(1,12),new Random().Next(1,28))
                },
                new StaffMember
                {
                    Name_User = "Jean Technicien",
                    Adresse_Mail = "jean@library.ca",
                    Num_Telephone = "613-555-0003",
                    Adresse = "789 Rue Slater, Ottawa, ON K1R 7X7",
                    Poste = "Technicien",
                    YearHired = 2020,
                    Password = "jean123",
 BirthDate = new DateTime(new Random().Next((DateTime.Now.Year-100),(DateTime.Now.Year-12)),new Random().Next(1,12),new Random().Next(1,28))
                }
            };

            foreach (var staff in staffMembers)
            {
                await context.StaffMembers.AddAsync(staff);
                await context.SaveChangesAsync(); // ID attribué par la base
                context.GenerateReferences();     // Reference calculée (Ref_Staff)
                await context.SaveChangesAsync(); // Reference persistée
            }

            Console.WriteLine($"✓ {staffMembers.Length} membres du personnel créés");
        }

        private static async Task CreateTestSubscribersAsync(LibraryDbContext context)
        {
            var random = new Random(42);
            var firstNames = new[] { "Sophie", "Thomas", "Julie", "Marc", "Claire", "Pierre", "Anne", "Luc", "Emma", "Alexandre", "Sarah", "Nicolas", "Isabelle", "François", "Catherine" };
            var lastNames = new[] { "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand", "Leroy", "Moreau", "Simon", "Laurent", "Lefebvre", "Michel", "Garcia" };

            for (int i = 0; i < 50; i++)
            {
                var first = firstNames[random.Next(firstNames.Length)];
                var last = lastNames[random.Next(lastNames.Length)];

                var subscriber = new Subscriber
                {
                    Name_User = $"{first} {last}",
                    Adresse_Mail = $"{first.ToLower()}.{last.ToLower()}{i}@email.ca",
                    Num_Telephone = $"613-555-{1000 + i:D4}",
                    Adresse = $"{100 + i * 10} Rue {last}, Ottawa, ON",
                    Fidelity = (decimal)(2 + random.NextDouble() * 6),
                    Password = $"{first.ToLower()}123",
                    BirthDate = new DateTime(new Random().Next((DateTime.Now.Year - 100), (DateTime.Now.Year - 12)), new Random().Next(1, 13), new Random().Next(1, 28))

                };

                await context.Subscribers.AddAsync(subscriber);
                await context.SaveChangesWithReferencesAsync(); // ✅ Un par un

                // Petit délai pour garantir l'unicité du timestamp
                await Task.Delay(10); // 10ms entre chaque création
            }

            Console.WriteLine($"✓ 30 abonnés créés");
        }

        private static async Task CreateTestBooksAsync(LibraryDbContext context)
        {
            // ⚠️ Ici tu remets ton tableau géant "books" :
            // var books = new[] { new Book { ... }, ... };
            var books = new[]
   {
    // 📚 LITTÉRATURE FRANÇAISE CLASSIQUE (15 livres)
    new Book
    {
        Title = "Le Petit Prince",
        Author = "Antoine de Saint-Exupéry",
        ISBN = "978-2070612758(2)",
        Category = "Classique",
        Description = "Un conte poétique et philosophique sur l'amitié, l'amour et la perte.",
        PublishDate = new DateTime(1943, 4, 6),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070612758-L.jpg"
    },
    new Book
    {
        Title = "Les Misérables",
        Author = "Victor Hugo",
        ISBN = "978-2253096337",
        Category = "Classique",
        Description = "Le chef-d'œuvre de Victor Hugo sur la rédemption et la justice sociale.",
        PublishDate = new DateTime(1862, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253096337-L.jpg"
    },
    new Book
    {
        Title = "L'Étranger",
        Author = "Albert Camus",
        ISBN = "978-2070360024(2)",
        Category = "Roman",
        Description = "Un roman existentiel sur l'absurde et la condition humaine.",
        PublishDate = new DateTime(1942, 5, 19),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070360024-L.jpg"
    },
    new Book
    {
        Title = "Madame Bovary",
        Author = "Gustave Flaubert",
        ISBN = "978-2253004820",
        Category = "Classique",
        Description = "Portrait tragique d'Emma Bovary et de ses rêves déçus.",
        PublishDate = new DateTime(1857, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004820-L.jpg"
    },
    new Book
    {
        Title = "Le Rouge et le Noir",
        Author = "Stendhal",
        ISBN = "978-2253085928",
        Category = "Classique",
        Description = "L'ascension de Julien Sorel dans la société française du XIXe siècle.",
        PublishDate = new DateTime(1830, 11, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253085928-L.jpg"
    },
    new Book
    {
        Title = "La Peste",
        Author = "Albert Camus",
        ISBN = "978-2070360420",
        Category = "Roman",
        Description = "Une allégorie de l'absurde face à une épidémie à Oran.",
        PublishDate = new DateTime(1947, 6, 10),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070360420-L.jpg"
    },
    new Book
    {
        Title = "Germinal",
        Author = "Émile Zola",
        ISBN = "978-2253004226",
        Category = "Classique",
        Description = "La lutte des mineurs dans le nord de la France au XIXe siècle.",
        PublishDate = new DateTime(1885, 3, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004226-L.jpg"
    },
    new Book
    {
        Title = "Notre-Dame de Paris",
        Author = "Victor Hugo",
        ISBN = "978-2253096344",
        Category = "Classique",
        Description = "L'histoire tragique de Quasimodo et Esmeralda dans le Paris médiéval.",
        PublishDate = new DateTime(1831, 3, 16),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253096344-L.jpg"
    },
    new Book
    {
        Title = "Le Père Goriot",
        Author = "Honoré de Balzac",
        ISBN = "978-2253085768",
        Category = "Classique",
        Description = "Le sacrifice d'un père pour ses filles ingrates dans la Comédie humaine.",
        PublishDate = new DateTime(1835, 3, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253085768-L.jpg"
    },
    new Book
    {
        Title = "Les Fleurs du mal",
        Author = "Charles Baudelaire",
        ISBN = "978-2070413437",
        Category = "Poésie",
        Description = "Recueil de poèmes révolutionnaire qui explore le spleen et l'idéal.",
        PublishDate = new DateTime(1857, 6, 25),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070413437-L.jpg"
    },
    new Book
    {
        Title = "Voyage au bout de la nuit",
        Author = "Louis-Ferdinand Céline",
        ISBN = "978-2070360024",
        Category = "Roman",
        Description = "Un voyage nihiliste à travers l'horreur de la guerre et de la vie moderne.",
        PublishDate = new DateTime(1932, 10, 15),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070360024-L.jpg"
    },
    new Book
    {
        Title = "Bel-Ami",
        Author = "Guy de Maupassant",
        ISBN = "978-2253004721",
        Category = "Classique",
        Description = "L'ascension sociale de Georges Duroy dans le Paris du Second Empire.",
        PublishDate = new DateTime(1885, 5, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004721-L.jpg"
    },
    new Book
    {
        Title = "Les Contemplations",
        Author = "Victor Hugo",
        ISBN = "978-2253006473",
        Category = "Poésie",
        Description = "Recueil poétique sur l'amour, la mort et le deuil de sa fille Léopoldine.",
        PublishDate = new DateTime(1856, 4, 23),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253006473-L.jpg"
    },
    new Book
    {
        Title = "Candide",
        Author = "Voltaire",
        ISBN = "978-2253096580",
        Category = "Classique",
        Description = "Conte philosophique satirique sur l'optimisme aveugle.",
        PublishDate = new DateTime(1759, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253096580-L.jpg"
    },
    new Book
    {
        Title = "Les Liaisons dangereuses",
        Author = "Pierre Choderlos de Laclos",
        ISBN = "978-2253004011",
        Category = "Classique",
        Description = "Roman épistolaire sur la séduction et la manipulation dans l'aristocratie.",
        PublishDate = new DateTime(1782, 3, 23),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004011-L.jpg"
    },

    // 📖 LITTÉRATURE FRANÇAISE CONTEMPORAINE (15 livres)
    new Book
    {
        Title = "L'Alchimiste",
        Author = "Paulo Coelho",
        ISBN = "978-2290331767",
        Category = "Roman",
        Description = "Un conte inspirant sur la quête de sa légende personnelle et du trésor intérieur.",
        PublishDate = new DateTime(1988, 1, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290331767-L.jpg"
    },
    new Book
    {
        Title = "La Vérité sur l'Affaire Harry Quebert",
        Author = "Joël Dicker",
        ISBN = "978-2877067447",
        Category = "Policier",
        Description = "Un thriller littéraire sur un meurtre mystérieux dans une petite ville américaine.",
        PublishDate = new DateTime(2012, 9, 6),
        Publisher = "De Fallois",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782877067447-L.jpg"
    },
    new Book
    {
        Title = "Les Gratitudes",
        Author = "Delphine de Vigan",
        ISBN = "978-2709662826",
        Category = "Roman",
        Description = "Un roman poignant sur la maladie d'Alzheimer et la reconnaissance.",
        PublishDate = new DateTime(2019, 8, 21),
        Publisher = "JC Lattès",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782709662826-L.jpg"
    },
    new Book
    {
        Title = "Chanson douce",
        Author = "Leïla Slimani",
        ISBN = "978-2070179688",
        Category = "Roman",
        Description = "Prix Goncourt 2016 - Un thriller psychologique glaçant sur une nounou.",
        PublishDate = new DateTime(2016, 8, 18),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070179688-L.jpg"
    },
    new Book
    {
        Title = "Le Grand Maulnes",
        Author = "Alain-Fournier",
        ISBN = "978-2253004752",
        Category = "Classique",
        Description = "Un roman initiatique sur l'adolescence et la quête du bonheur perdu.",
        PublishDate = new DateTime(1913, 10, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004752-L.jpg"
    },
    new Book
    {
        Title = "Millénium - Les hommes qui n'aimaient pas les femmes",
        Author = "Stieg Larsson",
        ISBN = "978-2266204675",
        Category = "Policier",
        Description = "Le thriller suédois culte avec Lisbeth Salander et Mikael Blomkvist.",
        PublishDate = new DateTime(2005, 8, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266204675-L.jpg"
    },
    new Book
    {
        Title = "Vernon Subutex, tome 1",
        Author = "Virginie Despentes",
        ISBN = "978-2246857389",
        Category = "Roman",
        Description = "Portrait d'un disquaire déchu dans le Paris contemporain.",
        PublishDate = new DateTime(2015, 1, 7),
        Publisher = "Grasset",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782246857389-L.jpg"
    },
    new Book
    {
        Title = "La Liste de mes envies",
        Author = "Grégoire Delacourt",
        ISBN = "978-2709643320",
        Category = "Roman",
        Description = "Une mercière gagne au loto - Sa vie va-t-elle changer?",
        PublishDate = new DateTime(2012, 8, 29),
        Publisher = "JC Lattès",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782709643320-L.jpg"
    },
    new Book
    {
        Title = "Ensemble, c'est tout",
        Author = "Anna Gavalda",
        ISBN = "978-2290349762",
        Category = "Roman",
        Description = "L'histoire touchante de quatre personnages qui forment une famille de cœur.",
        PublishDate = new DateTime(2004, 9, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290349762-L.jpg"
    },
    new Book
    {
        Title = "Stupeur et Tremblements",
        Author = "Amélie Nothomb",
        ISBN = "978-2253150718",
        Category = "Roman",
        Description = "L'expérience surréaliste d'une jeune Belge dans une entreprise japonaise.",
        PublishDate = new DateTime(1999, 8, 25),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253150718-L.jpg"
    },
    new Book
    {
        Title = "Au revoir là-haut",
        Author = "Pierre Lemaitre",
        ISBN = "978-2253194996",
        Category = "Roman",
        Description = "Prix Goncourt 2013 - Une fresque sur les gueules cassées après 14-18.",
        PublishDate = new DateTime(2013, 8, 21),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253194996-L.jpg"
    },
    new Book
    {
        Title = "Les Fourmis",
        Author = "Bernard Werber",
        ISBN = "978-2253063339",
        Category = "ScienceFiction",
        Description = "Une épopée fascinante dans le monde parallèle des fourmis.",
        PublishDate = new DateTime(1991, 3, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253063339-L.jpg"
    },
    new Book
    {
        Title = "Il est grand temps de rallumer les étoiles",
        Author = "Virginie Grimaldi",
        ISBN = "978-2213710785",
        Category = "Roman",
        Description = "Un roman feel-good sur trois personnages qui se reconstruisent.",
        PublishDate = new DateTime(2018, 8, 22),
        Publisher = "Fayard",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782213710785-L.jpg"
    },
    new Book
    {
        Title = "La Vie devant soi",
        Author = "Romain Gary (Émile Ajar)",
        ISBN = "978-2070369300",
        Category = "Roman",
        Description = "Prix Goncourt 1975 - L'histoire touchante d'un enfant arabe et d'une vieille juive.",
        PublishDate = new DateTime(1975, 9, 3),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070369300-L.jpg"
    },
    new Book
    {
        Title = "Nuit blanche au musée",
        Author = "Olivier Norek",
        ISBN = "978-2749164908",
        Category = "Policier",
        Description = "Un thriller haletant se déroulant pendant une nuit au musée.",
        PublishDate = new DateTime(2021, 2, 18),
        Publisher = "Michel Lafon",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782749164908-L.jpg"
    },

    // 🚀 SCIENCE-FICTION (12 livres)
    new Book
    {
        Title = "Dune",
        Author = "Frank Herbert",
        ISBN = "978-2266320481",
        Category = "ScienceFiction",
        Description = "L'épopée interplanétaire légendaire sur la planète Arrakis.",
        PublishDate = new DateTime(1965, 8, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266320481-L.jpg"
    },
    new Book
    {
        Title = "Fondation",
        Author = "Isaac Asimov",
        ISBN = "978-2070415816",
        Category = "ScienceFiction",
        Description = "Le cycle mythique de la Fondation - Premier tome de la saga.",
        PublishDate = new DateTime(1951, 1, 1),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070415816-L.jpg"
    },
    new Book
    {
        Title = "Neuromancien",
        Author = "William Gibson",
        ISBN = "978-2290343234",
        Category = "ScienceFiction",
        Description = "Le roman fondateur du cyberpunk qui a révolutionné la science-fiction.",
        PublishDate = new DateTime(1984, 7, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290343234-L.jpg"
    },
    new Book
    {
        Title = "1984",
        Author = "George Orwell",
        ISBN = "978-2070368228",
        Category = "ScienceFiction",
        Description = "Une dystopie totalitaire devenue un classique de la littérature mondiale.",
        PublishDate = new DateTime(1949, 6, 8),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070368228-L.jpg"
    },
    new Book
    {
        Title = "Le Meilleur des mondes",
        Author = "Aldous Huxley",
        ISBN = "978-2266283892",
        Category = "ScienceFiction",
        Description = "Dystopie visionnaire sur une société parfaitement contrôlée.",
        PublishDate = new DateTime(1932, 1, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266283892-L.jpg"
    },
    new Book
    {
        Title = "Fahrenheit 451",
        Author = "Ray Bradbury",
        ISBN = "978-2070360673",
        Category = "ScienceFiction",
        Description = "Dans un futur où les livres sont interdits et brûlés.",
        PublishDate = new DateTime(1953, 10, 19),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070360673-L.jpg"
    },
    new Book
    {
        Title = "La Main gauche de la nuit",
        Author = "Ursula K. Le Guin",
        ISBN = "978-2221116944",
        Category = "ScienceFiction",
        Description = "Une exploration fascinante du genre sur la planète Gethen.",
        PublishDate = new DateTime(1969, 3, 1),
        Publisher = "Robert Laffont",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782221116944-L.jpg"
    },
    new Book
    {
        Title = "Le Guide du voyageur galactique",
        Author = "Douglas Adams",
        ISBN = "978-2070612208",
        Category = "ScienceFiction",
        Description = "Une comédie de science-fiction déjantée et culte.",
        PublishDate = new DateTime(1979, 10, 12),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070612208-L.jpg"
    },
    new Book
    {
        Title = "Ubik",
        Author = "Philip K. Dick",
        ISBN = "978-2290312995",
        Category = "ScienceFiction",
        Description = "Un chef-d'œuvre paranoïaque sur la réalité et l'illusion.",
        PublishDate = new DateTime(1969, 6, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290312995-L.jpg"
    },
    new Book
    {
        Title = "Le Problème à trois corps",
        Author = "Liu Cixin",
        ISBN = "978-2330054052",
        Category = "ScienceFiction",
        Description = "Le premier contact avec une civilisation extraterrestre hostile.",
        PublishDate = new DateTime(2008, 1, 1),
        Publisher = "Actes Sud",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782330054052-L.jpg"
    },
    new Book
    {
        Title = "Les Robots",
        Author = "Isaac Asimov",
        ISBN = "978-2290315415",
        Category = "ScienceFiction",
        Description = "Les célèbres nouvelles sur les trois lois de la robotique.",
        PublishDate = new DateTime(1950, 12, 2),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290315415-L.jpg"
    },
    new Book
    {
        Title = "Chroniques martiennes",
        Author = "Ray Bradbury",
        ISBN = "978-2070360697",
        Category = "ScienceFiction",
        Description = "Recueil poétique de nouvelles sur la colonisation de Mars.",
        PublishDate = new DateTime(1950, 5, 4),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070360697-L.jpg"
    },

    // 🧙‍♂️ FANTASY (10 livres)
    new Book
    {
        Title = "Le Seigneur des Anneaux - La Communauté de l'Anneau",
        Author = "J.R.R. Tolkien",
        ISBN = "978-2266154345",
        Category = "Fantasy",
        Description = "Le début de l'épopée légendaire en Terre du Milieu.",
        PublishDate = new DateTime(1954, 7, 29),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266154345-L.jpg"
    },
    new Book
    {
        Title = "Harry Potter à l'école des sorciers",
        Author = "J.K. Rowling",
        ISBN = "978-2070643028",
        Category = "Fantasy",
        Description = "Le premier tome des aventures du jeune sorcier Harry Potter à Poudlard.",
        PublishDate = new DateTime(1997, 6, 26),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 10,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070643028-L.jpg"
    },
    new Book
    {
        Title = "Le Nom du Vent",
        Author = "Patrick Rothfuss",
        ISBN = "978-2352943693",
        Category = "Fantasy",
        Description = "Les chroniques du tueur de roi - Premier jour.",
        PublishDate = new DateTime(2007, 3, 27),
        Publisher = "Bragelonne",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782352943693-L.jpg"
    },
    new Book
    {
        Title = "Le Trône de Fer",
        Author = "George R.R. Martin",
        ISBN = "978-2290053690",
        Category = "Fantasy",
        Description = "Le premier tome de l'épopée médiévale-fantastique de Westeros.",
        PublishDate = new DateTime(1996, 8, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290053690-L.jpg"
    },
    new Book
    {
        Title = "Le Hobbit",
        Author = "J.R.R. Tolkien",
        ISBN = "978-2266154321",
        Category = "Fantasy",
        Description = "L'aventure de Bilbon Sacquet avant Le Seigneur des Anneaux.",
        PublishDate = new DateTime(1937, 9, 21),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266154321-L.jpg"
    },
    new Book
    {
        Title = "Assassin Royal - L'Apprenti assassin",
        Author = "Robin Hobb",
        ISBN = "978-2290316849",
        Category = "Fantasy",
        Description = "Les débuts de FitzChevalerie Loinvoyant comme assassin royal.",
        PublishDate = new DateTime(1995, 5, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290316849-L.jpg"
    },
    new Book
    {
        Title = "La Roue du Temps - L'Œil du Monde",
        Author = "Robert Jordan",
        ISBN = "978-2266138840",
        Category = "Fantasy",
        Description = "Le début d'une saga fantasy épique de 14 tomes.",
        PublishDate = new DateTime(1990, 1, 15),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266138840-L.jpg"
    },
    new Book
    {
        Title = "Neverwhere",
        Author = "Neil Gaiman",
        ISBN = "978-2290359556",
        Category = "Fantasy",
        Description = "L'urban fantasy dans le Londres souterrain magique.",
        PublishDate = new DateTime(1996, 9, 16),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290359556-L.jpg"
    },
    new Book
    {
        Title = "Gagner la guerre",
        Author = "Jean-Philippe Jaworski",
        ISBN = "978-2352943587",
        Category = "Fantasy",
        Description = "Un roman de fantasy politique dans une cité-État fascinante.",
        PublishDate = new DateTime(2009, 10, 15),
        Publisher = "Bragelonne",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782352943587-L.jpg"
    },
    new Book
    {
        Title = "Les Annales du Disque-Monde - La Huitième Couleur",
        Author = "Terry Pratchett",
        ISBN = "978-2266154284",
        Category = "Fantasy",
        Description = "Le premier tome de l'univers comique du Disque-Monde.",
        PublishDate = new DateTime(1983, 11, 24),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266154284-L.jpg"
    },

    // 🔍 POLICIER & THRILLER (12 livres)
    new Book
    {
        Title = "Gone Girl",
        Author = "Gillian Flynn",
        ISBN = "978-2355845642",
        Category = "Policier",
        Description = "Un thriller psychologique haletant sur un mariage qui vire au cauchemar.",
        PublishDate = new DateTime(2012, 6, 5),
        Publisher = "Sonatine",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782355845642-L.jpg"
    },
    new Book
    {
        Title = "Le Silence des agneaux",
        Author = "Thomas Harris",
        ISBN = "978-2253061779",
        Category = "Policier",
        Description = "Le face-à-face légendaire entre Clarice Starling et Hannibal Lecter.",
        PublishDate = new DateTime(1988, 10, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253061779-L.jpg"
    },
    new Book
    {
        Title = "Les Rivières pourpres",
        Author = "Jean-Christophe Grangé",
        ISBN = "978-2253150510",
        Category = "Policier",
        Description = "Un thriller glacial dans les Alpes françaises.",
        PublishDate = new DateTime(1998, 9, 3),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253150510-L.jpg"
    },
    new Book
    {
        Title = "Le Parfum",
        Author = "Patrick Süskind",
        ISBN = "978-2253033424",
        Category = "Policier",
        Description = "L'histoire fascinante d'un génie du parfum meurtrier au XVIIIe siècle.",
        PublishDate = new DateTime(1985, 3, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253033424-L.jpg"
    },
    new Book
    {
        Title = "Le Chien des Baskerville",
        Author = "Arthur Conan Doyle",
        ISBN = "978-2253002482",
        Category = "Policier",
        Description = "La plus célèbre enquête de Sherlock Holmes dans la lande anglaise.",
        PublishDate = new DateTime(1902, 4, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253002482-L.jpg"
    },
    new Book
    {
        Title = "Le Crime de l'Orient-Express",
        Author = "Agatha Christie",
        ISBN = "978-2253011835",
        Category = "Policier",
        Description = "Hercule Poirot enquête sur un meurtre dans le célèbre train.",
        PublishDate = new DateTime(1934, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253011835-L.jpg"
    },
    new Book
    {
        Title = "La Fille du train",
        Author = "Paula Hawkins",
        ISBN = "978-2355846045",
        Category = "Policier",
        Description = "Un thriller psychologique addictif vu par trois femmes.",
        PublishDate = new DateTime(2015, 1, 13),
        Publisher = "Sonatine",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782355846045-L.jpg"
    },
    new Book
    {
        Title = "Shutter Island",
        Author = "Dennis Lehane",
        ISBN = "978-2743621117",
        Category = "Policier",
        Description = "Un marshal enquête sur une disparition dans un hôpital psychiatrique.",
        PublishDate = new DateTime(2003, 4, 15),
        Publisher = "Rivages",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782743621117-L.jpg"
    },
    new Book
    {
        Title = "Le Code Da Vinci",
        Author = "Dan Brown",
        ISBN = "978-2253172451",
        Category = "Policier",
        Description = "Une enquête haletante sur les secrets du christianisme.",
        PublishDate = new DateTime(2003, 3, 18),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253172451-L.jpg"
    },
    new Book
    {
        Title = "Alex Cross",
        Author = "James Patterson",
        ISBN = "978-2266198165",
        Category = "Policier",
        Description = "Le premier tome de la série avec le détective Alex Cross.",
        PublishDate = new DateTime(1993, 1, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266198165-L.jpg"
    },
    new Book
    {
        Title = "Le Huit",
        Author = "Katherine Neville",
        ISBN = "978-2253047995",
        Category = "Policier",
        Description = "Un thriller historique autour d'un jeu d'échecs légendaire.",
        PublishDate = new DateTime(1988, 9, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253047995-L.jpg"
    },
    new Book
    {
        Title = "American Psycho",
        Author = "Bret Easton Ellis",
        ISBN = "978-2266204170",
        Category = "Policier",
        Description = "Portrait glaçant d'un serial killer yuppie dans les années 80.",
        PublishDate = new DateTime(1991, 3, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266204170-L.jpg"
    },

    // 😱 HORREUR (8 livres)
    new Book
    {
        Title = "Shining",
        Author = "Stephen King",
        ISBN = "978-2253151340",
        Category = "Horreur",
        Description = "Un roman d'horreur psychologique se déroulant dans l'hôtel Overlook isolé.",
        PublishDate = new DateTime(1977, 1, 28),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253151340-L.jpg"
    },
    new Book
    {
        Title = "Ça",
        Author = "Stephen King",
        ISBN = "978-2253151357",
        Category = "Horreur",
        Description = "Sept enfants affrontent une créature maléfique à Derry, Maine.",
        PublishDate = new DateTime(1986, 9, 15),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253151357-L.jpg"
    },
    new Book
    {
        Title = "Dracula",
        Author = "Bram Stoker",
        ISBN = "978-2253006329",
        Category = "Horreur",
        Description = "Le roman gothique qui a créé le mythe moderne du vampire.",
        PublishDate = new DateTime(1897, 5, 26),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253006329-L.jpg"
    },
    new Book
    {
        Title = "Frankenstein",
        Author = "Mary Shelley",
        ISBN = "978-2253006480",
        Category = "Horreur",
        Description = "Le roman fondateur de la science-fiction et de l'horreur gothique.",
        PublishDate = new DateTime(1818, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253006480-L.jpg"
    },
    new Book
    {
        Title = "L'Exorciste",
        Author = "William Peter Blatty",
        ISBN = "978-2253149842",
        Category = "Horreur",
        Description = "Le roman d'horreur terrifiant sur une possession démoniaque.",
        PublishDate = new DateTime(1971, 5, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253149842-L.jpg"
    },
    new Book
    {
        Title = "Carrie",
        Author = "Stephen King",
        ISBN = "978-2253004936",
        Category = "Horreur",
        Description = "Le premier roman de Stephen King sur une adolescente aux pouvoirs télékynétiques.",
        PublishDate = new DateTime(1974, 4, 5),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253004936-L.jpg"
    },
    new Book
    {
        Title = "Pet Sematary",
        Author = "Stephen King",
        ISBN = "978-2253151364",
        Category = "Horreur",
        Description = "Un cimetière indien où les morts reviennent... changés.",
        PublishDate = new DateTime(1983, 11, 14),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253151364-L.jpg"
    },
    new Book
    {
        Title = "Rosemary's Baby",
        Author = "Ira Levin",
        ISBN = "978-2253149811",
        Category = "Horreur",
        Description = "Un thriller paranoïaque sur une grossesse diabolique à New York.",
        PublishDate = new DateTime(1967, 4, 12),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253149811-L.jpg"
    },

    // 🧠 SCIENCES & SAVOIRS (8 livres)
    new Book
    {
        Title = "Sapiens: Une brève histoire de l'humanité",
        Author = "Yuval Noah Harari",
        ISBN = "978-2226257017",
        Category = "Histoire",
        Description = "Un panorama fascinant de l'évolution de l'espèce humaine et de ses structures sociales.",
        PublishDate = new DateTime(2011, 1, 1),
        Publisher = "Albin Michel",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782226257017-L.jpg"
    },
    new Book
    {
        Title = "Une brève histoire du temps",
        Author = "Stephen Hawking",
        ISBN = "978-2081379602",
        Category = "Science",
        Description = "Du Big Bang aux trous noirs - Les grandes théories de l'univers expliquées simplement.",
        PublishDate = new DateTime(1988, 4, 1),
        Publisher = "Flammarion",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782081379602-L.jpg"
    },
    new Book
    {
        Title = "Le Gène égoïste",
        Author = "Richard Dawkins",
        ISBN = "978-2738105004",
        Category = "Science",
        Description = "La théorie de l'évolution revisitée du point de vue du gène.",
        PublishDate = new DateTime(1976, 1, 1),
        Publisher = "Odile Jacob",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782738105004-L.jpg"
    },
    new Book
    {
        Title = "Homo Deus",
        Author = "Yuval Noah Harari",
        ISBN = "978-2226393876",
        Category = "Histoire",
        Description = "Une brève histoire du futur et de l'évolution de l'humanité.",
        PublishDate = new DateTime(2015, 1, 1),
        Publisher = "Albin Michel",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782226393876-L.jpg"
    },
    new Book
    {
        Title = "Cosmos",
        Author = "Carl Sagan",
        ISBN = "978-2757803523",
        Category = "Science",
        Description = "Un voyage époustouflant à travers l'univers et notre place dans celui-ci.",
        PublishDate = new DateTime(1980, 9, 28),
        Publisher = "Points",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782757803523-L.jpg"
    },
    new Book
    {
        Title = "Le Capital au XXIe siècle",
        Author = "Thomas Piketty",
        ISBN = "978-2021082289",
        Category = "Économie",
        Description = "Une analyse magistrale des inégalités économiques et de leur évolution historique.",
        PublishDate = new DateTime(2013, 8, 25),
        Publisher = "Seuil",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782021082289-L.jpg"
    },
    new Book
    {
        Title = "Pourquoi nous dormons",
        Author = "Matthew Walker",
        ISBN = "978-2221240571",
        Category = "Science",
        Description = "Les dernières découvertes scientifiques sur le sommeil et les rêves.",
        PublishDate = new DateTime(2017, 10, 3),
        Publisher = "Robert Laffont",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782221240571-L.jpg"
    },
    new Book
    {
        Title = "L'Origine des espèces",
        Author = "Charles Darwin",
        ISBN = "978-2081379640",
        Category = "Science",
        Description = "L'œuvre révolutionnaire qui a changé notre compréhension de la vie.",
        PublishDate = new DateTime(1859, 11, 24),
        Publisher = "Flammarion",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782081379640-L.jpg"
    },

    // 💻 INFORMATIQUE & TECHNOLOGIE (6 livres)
    new Book
    {
        Title = "Clean Code",
        Author = "Robert C. Martin",
        ISBN = "978-0132350884",
        Category = "Informatique",
        Description = "L'art d'écrire du code propre et maintenable - Le guide de référence.",
        PublishDate = new DateTime(2008, 8, 1),
        Publisher = "Prentice Hall",
        Language = "Anglais",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg"
    },
    new Book
    {
        Title = "Design Patterns",
        Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
        ISBN = "978-0201633610",
        Category = "Informatique",
        Description = "Les patterns de conception orientée objet - Le livre du Gang of Four.",
        PublishDate = new DateTime(1994, 10, 31),
        Publisher = "Addison-Wesley",
        Language = "Anglais",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201633610-L.jpg"
    },
    new Book
    {
        Title = "The Pragmatic Programmer",
        Author = "Andrew Hunt & David Thomas",
        ISBN = "978-0135957059",
        Category = "Informatique",
        Description = "De l'artisan au maître - Les meilleures pratiques de programmation.",
        PublishDate = new DateTime(1999, 10, 30),
        Publisher = "Addison-Wesley",
        Language = "Anglais",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780135957059-L.jpg"
    },
    new Book
    {
        Title = "Introduction to Algorithms",
        Author = "Thomas H. Cormen",
        ISBN = "978-0262033848",
        Category = "Informatique",
        Description = "Le manuel de référence sur les algorithmes et structures de données.",
        PublishDate = new DateTime(2009, 7, 31),
        Publisher = "MIT Press",
        Language = "Anglais",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780262033848-L.jpg"
    },
    new Book
    {
        Title = "Cracking the Coding Interview",
        Author = "Gayle Laakmann McDowell",
        ISBN = "978-0984782857",
        Category = "Informatique",
        Description = "189 questions de programmation pour réussir vos entretiens techniques.",
        PublishDate = new DateTime(2015, 7, 1),
        Publisher = "CareerCup",
        Language = "Anglais",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780984782857-L.jpg"
    },
    new Book
    {
        Title = "You Don't Know JS",
        Author = "Kyle Simpson",
        ISBN = "978-1491904244",
        Category = "Informatique",
        Description = "Plongée profonde dans les mécanismes de JavaScript.",
        PublishDate = new DateTime(2015, 3, 29),
        Publisher = "O'Reilly",
        Language = "Anglais",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9781491904244-L.jpg"
    },

    // 📖 BANDE DESSINÉE (8 livres)
    new Book
    {
        Title = "Astérix le Gaulois",
        Author = "René Goscinny & Albert Uderzo",
        ISBN = "978-2012101319",
        Category = "BandeDessinee",
        Description = "Le premier tome des aventures d'Astérix et Obélix en Gaule.",
        PublishDate = new DateTime(1961, 1, 1),
        Publisher = "Hachette",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782012101319-L.jpg"
    },
    new Book
    {
        Title = "Tintin au Tibet",
        Author = "Hergé",
        ISBN = "978-2203001190",
        Category = "BandeDessinee",
        Description = "L'une des plus belles et émouvantes aventures de Tintin.",
        PublishDate = new DateTime(1960, 1, 1),
        Publisher = "Casterman",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782203001190-L.jpg"
    },
    new Book
    {
        Title = "Le Secret de la Licorne",
        Author = "Hergé",
        ISBN = "978-2203001121",
        Category = "BandeDessinee",
        Description = "Tintin part à la recherche du trésor de Rackham le Rouge.",
        PublishDate = new DateTime(1943, 1, 1),
        Publisher = "Casterman",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782203001121-L.jpg"
    },
    new Book
    {
        Title = "Lucky Luke - La Diligence",
        Author = "Morris & René Goscinny",
        ISBN = "978-2884710022",
        Category = "BandeDessinee",
        Description = "Le cow-boy solitaire protège une diligence dans le Far West.",
        PublishDate = new DateTime(1968, 1, 1),
        Publisher = "Dargaud",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782884710022-L.jpg"
    },
    new Book
    {
        Title = "Gaston - Gaffes à gogo",
        Author = "André Franquin",
        ISBN = "978-2800100005",
        Category = "BandeDessinee",
        Description = "Les gaffes légendaires de Gaston Lagaffe au bureau.",
        PublishDate = new DateTime(1963, 1, 1),
        Publisher = "Dupuis",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782800100005-L.jpg"
    },
    new Book
    {
        Title = "Spirou et Fantasio - Le Prisonnier du Bouddha",
        Author = "André Franquin",
        ISBN = "978-2800100401",
        Category = "BandeDessinee",
        Description = "Une aventure exotique de Spirou et Fantasio en Asie.",
        PublishDate = new DateTime(1960, 1, 1),
        Publisher = "Dupuis",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782800100401-L.jpg"
    },
    new Book
    {
        Title = "XIII - Le Jour du soleil noir",
        Author = "Jean Van Hamme & William Vance",
        ISBN = "978-2871290018",
        Category = "BandeDessinee",
        Description = "Le début de la série d'espionnage culte sur un amnésique.",
        PublishDate = new DateTime(1984, 1, 1),
        Publisher = "Dargaud",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782871290018-L.jpg"
    },
    new Book
    {
        Title = "Watchmen",
        Author = "Alan Moore & Dave Gibbons",
        ISBN = "978-2365770132",
        Category = "BandeDessinee",
        Description = "Le chef-d'œuvre qui a révolutionné la bande dessinée de super-héros.",
        PublishDate = new DateTime(1987, 9, 1),
        Publisher = "Urban Comics",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782365770132-L.jpg"
    },

    // 📚 MANGA (8 livres)
    new Book
    {
        Title = "One Piece - Tome 1",
        Author = "Eiichiro Oda",
        ISBN = "978-2723492997",
        Category = "Manga",
        Description = "Les aventures de Monkey D. Luffy à la recherche du One Piece commencent.",
        PublishDate = new DateTime(1997, 12, 24),
        Publisher = "Glénat",
        Language = "Français",
        Quantity = 12,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782723492997-L.jpg"
    },
    new Book
    {
        Title = "Naruto - Tome 1",
        Author = "Masashi Kishimoto",
        ISBN = "978-2871293392",
        Category = "Manga",
        Description = "Le début de l'aventure du jeune ninja Naruto Uzumaki.",
        PublishDate = new DateTime(1999, 9, 21),
        Publisher = "Kana",
        Language = "Français",
        Quantity = 10,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782871293392-L.jpg"
    },
    new Book
    {
        Title = "Death Note - Tome 1",
        Author = "Tsugumi Ohba & Takeshi Obata",
        ISBN = "978-2871299431",
        Category = "Manga",
        Description = "Light Yagami trouve un cahier qui tue toute personne dont on écrit le nom.",
        PublishDate = new DateTime(2003, 12, 1),
        Publisher = "Kana",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782871299431-L.jpg"
    },
    new Book
    {
        Title = "Dragon Ball - Tome 1",
        Author = "Akira Toriyama",
        ISBN = "978-2723415262",
        Category = "Manga",
        Description = "Les débuts de Son Goku dans sa quête des Dragon Balls.",
        PublishDate = new DateTime(1984, 12, 3),
        Publisher = "Glénat",
        Language = "Français",
        Quantity = 9,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782723415262-L.jpg"
    },
    new Book
    {
        Title = "L'Attaque des Titans - Tome 1",
        Author = "Hajime Isayama",
        ISBN = "978-2811603229",
        Category = "Manga",
        Description = "L'humanité lutte pour sa survie contre les Titans mangeurs d'hommes.",
        PublishDate = new DateTime(2009, 9, 9),
        Publisher = "Pika",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782811603229-L.jpg"
    },
    new Book
    {
        Title = "Fullmetal Alchemist - Tome 1",
        Author = "Hiromu Arakawa",
        ISBN = "978-2351420027",
        Category = "Manga",
        Description = "Deux frères alchimistes cherchent la Pierre Philosophale.",
        PublishDate = new DateTime(2001, 7, 12),
        Publisher = "Kurokawa",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782351420027-L.jpg"
    },
    new Book
    {
        Title = "Hunter × Hunter - Tome 1",
        Author = "Yoshihiro Togashi",
        ISBN = "978-2871293408",
        Category = "Manga",
        Description = "Gon part à l'aventure pour devenir Hunter et retrouver son père.",
        PublishDate = new DateTime(1998, 6, 4),
        Publisher = "Kana",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782871293408-L.jpg"
    },
    new Book
    {
        Title = "Tokyo Ghoul - Tome 1",
        Author = "Sui Ishida",
        ISBN = "978-2723498029",
        Category = "Manga",
        Description = "Ken Kaneki devient un hybride humain-goule dans Tokyo moderne.",
        PublishDate = new DateTime(2011, 9, 8),
        Publisher = "Glénat",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782723498029-L.jpg"
    },

    // 💭 PHILOSOPHIE & PSYCHOLOGIE (8 livres)
    new Book
    {
        Title = "Les Quatre Accords Toltèques",
        Author = "Don Miguel Ruiz",
        ISBN = "978-2883534094",
        Category = "Psychologie",
        Description = "La voie de la liberté personnelle à travers quatre principes de sagesse toltèque.",
        PublishDate = new DateTime(1997, 1, 1),
        Publisher = "Jouvence",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782883534094-L.jpg"
    },
    new Book
    {
        Title = "Le Monde de Sophie",
        Author = "Jostein Gaarder",
        ISBN = "978-2020235143",
        Category = "Philosophie",
        Description = "Une initiation ludique et passionnante à l'histoire de la philosophie.",
        PublishDate = new DateTime(1991, 1, 1),
        Publisher = "Seuil",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782020235143-L.jpg"
    },
    new Book
    {
        Title = "L'Art de la guerre",
        Author = "Sun Tzu",
        ISBN = "978-2081218055",
        Category = "Philosophie",
        Description = "Le traité de stratégie militaire le plus influent de tous les temps.",
        PublishDate = new DateTime(1, 1, 1),
        Publisher = "Flammarion",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782081218055-L.jpg"
    },
    new Book
    {
        Title = "Pensées pour moi-même",
        Author = "Marc Aurèle",
        ISBN = "978-2081277694",
        Category = "Philosophie",
        Description = "Les réflexions stoïciennes de l'empereur romain.",
        PublishDate = new DateTime(170, 1, 1),
        Publisher = "Flammarion",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782081277694-L.jpg"
    },
    new Book
    {
        Title = "Le Pouvoir du moment présent",
        Author = "Eckhart Tolle",
        ISBN = "978-2290020142",
        Category = "Psychologie",
        Description = "Guide spirituel pour vivre l'instant présent et trouver la paix intérieure.",
        PublishDate = new DateTime(1997, 1, 1),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290020142-L.jpg"
    },
    new Book
    {
        Title = "L'Existentialisme est un humanisme",
        Author = "Jean-Paul Sartre",
        ISBN = "978-2070329137",
        Category = "Philosophie",
        Description = "Conférence fondamentale sur l'existentialisme sartrien.",
        PublishDate = new DateTime(1946, 1, 1),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070329137-L.jpg"
    },
    new Book
    {
        Title = "Influence et manipulation",
        Author = "Robert Cialdini",
        ISBN = "978-2266190961",
        Category = "Psychologie",
        Description = "Les principes psychologiques de la persuasion et de l'influence.",
        PublishDate = new DateTime(1984, 1, 1),
        Publisher = "Pocket",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782266190961-L.jpg"
    },
    new Book
    {
        Title = "Ainsi parlait Zarathoustra",
        Author = "Friedrich Nietzsche",
        ISBN = "978-2253006534",
        Category = "Philosophie",
        Description = "Le chef-d'œuvre philosophique de Nietzsche sur le surhomme.",
        PublishDate = new DateTime(1883, 1, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253006534-L.jpg"
    },

    // 🎨 ARTS & CULTURE (5 livres)
    new Book
    {
        Title = "L'Histoire de l'Art",
        Author = "Ernst H. Gombrich",
        ISBN = "978-0714832470",
        Category = "Art",
        Description = "Un classique de la vulgarisation artistique, de la préhistoire à l'art moderne.",
        PublishDate = new DateTime(1950, 11, 1),
        Publisher = "Phaidon",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9780714832470-L.jpg"
    },
    new Book
    {
        Title = "La Petite Histoire de l'Art",
        Author = "Susie Hodge",
        ISBN = "978-2081422551",
        Category = "Art",
        Description = "50 œuvres emblématiques expliquées de manière accessible.",
        PublishDate = new DateTime(2017, 10, 4),
        Publisher = "Flammarion",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782081422551-L.jpg"
    },
    new Book
    {
        Title = "L'Art abstrait",
        Author = "Anna Moszynska",
        ISBN = "978-2878113433",
        Category = "Art",
        Description = "Une exploration complète de l'abstraction en peinture et sculpture.",
        PublishDate = new DateTime(1990, 1, 1),
        Publisher = "Thames & Hudson",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782878113433-L.jpg"
    },
    new Book
    {
        Title = "Comment parler de l'art du XXe siècle aux enfants",
        Author = "Françoise Barbe-Gall",
        ISBN = "978-2362900204",
        Category = "Art",
        Description = "Guide pédagogique pour découvrir l'art moderne avec les enfants.",
        PublishDate = new DateTime(2011, 10, 6),
        Publisher = "Le Baron perché",
        Language = "Français",
        Quantity = 3,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782362900204-L.jpg"
    },
    new Book
    {
        Title = "Petite histoire de la photographie",
        Author = "Walter Benjamin",
        ISBN = "978-2226257628",
        Category = "Photographie",
        Description = "Essai fondamental sur l'art photographique et sa reproductibilité.",
        PublishDate = new DateTime(1931, 1, 1),
        Publisher = "Allia",
        Language = "Français",
        Quantity = 2,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782226257628-L.jpg"
    },

    // 🌿 NATURE & VIE PRATIQUE (6 livres)
    new Book
    {
        Title = "Cuisine du Monde",
        Author = "Collectif",
        ISBN = "978-2017087742",
        Category = "Cuisine",
        Description = "Un tour du monde culinaire à travers 80 recettes emblématiques.",
        PublishDate = new DateTime(2019, 10, 15),
        Publisher = "Hachette",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782017087742-L.jpg"
    },
    new Book
    {
        Title = "Le Grand Livre Marabout de la Cuisine Facile",
        Author = "Collectif Marabout",
        ISBN = "978-2501065078",
        Category = "Cuisine",
        Description = "900 recettes simples pour cuisiner au quotidien.",
        PublishDate = new DateTime(2010, 9, 1),
        Publisher = "Marabout",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782501065078-L.jpg"
    },
    new Book
    {
        Title = "La Permaculture au jardin mois par mois",
        Author = "Damien Dekarz",
        ISBN = "978-2317021503",
        Category = "Jardinage",
        Description = "Guide pratique pour créer un jardin en permaculture.",
        PublishDate = new DateTime(2019, 3, 14),
        Publisher = "Rustica",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782317021503-L.jpg"
    },
    new Book
    {
        Title = "Le Potager du paresseux",
        Author = "Didier Helmstetter",
        ISBN = "978-2815312196",
        Category = "Jardinage",
        Description = "Produire des légumes bio en abondance sans efforts.",
        PublishDate = new DateTime(2017, 2, 24),
        Publisher = "Tana",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782815312196-L.jpg"
    },
    new Book
    {
        Title = "La Vie secrète des arbres",
        Author = "Peter Wohlleben",
        ISBN = "978-2290147436",
        Category = "Nature",
        Description = "Découverte fascinante de la communication et de la vie sociale des arbres.",
        PublishDate = new DateTime(2015, 5, 25),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290147436-L.jpg"
    },
    new Book
    {
        Title = "Zéro déchet",
        Author = "Béa Johnson",
        ISBN = "978-2290121207",
        Category = "Environnement",
        Description = "Le guide pour adopter un mode de vie zéro déchet.",
        PublishDate = new DateTime(2013, 4, 9),
        Publisher = "J'ai Lu",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782290121207-L.jpg"
    },

    // 👶 JEUNESSE (6 livres)
    new Book
    {
        Title = "Le Petit Nicolas",
        Author = "René Goscinny & Jean-Jacques Sempé",
        ISBN = "978-2070612758",
        Category = "Jeunesse",
        Description = "Les aventures drôles et tendres du Petit Nicolas et ses copains.",
        PublishDate = new DateTime(1960, 3, 29),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 7,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070612758-L.jpg"
    },
    new Book
    {
        Title = "Le Petit Prince",
        Author = "Antoine de Saint-Exupéry",
        ISBN = "978-2070408504",
        Category = "Jeunesse",
        Description = "Le conte philosophique pour enfants et adultes le plus célèbre.",
        PublishDate = new DateTime(1943, 4, 6),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 8,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070408504-L.jpg"
    },
    new Book
    {
        Title = "Charlie et la Chocolaterie",
        Author = "Roald Dahl",
        ISBN = "978-2070612765",
        Category = "Jeunesse",
        Description = "L'aventure merveilleuse de Charlie dans la fabrique de Willy Wonka.",
        PublishDate = new DateTime(1964, 1, 17),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 6,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070612765-L.jpg"
    },
    new Book
    {
        Title = "Matilda",
        Author = "Roald Dahl",
        ISBN = "978-2070601608",
        Category = "Jeunesse",
        Description = "L'histoire d'une petite fille surdouée aux pouvoirs extraordinaires.",
        PublishDate = new DateTime(1988, 10, 1),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070601608-L.jpg"
    },
    new Book
    {
        Title = "Le Lion",
        Author = "Joseph Kessel",
        ISBN = "978-2070612826",
        Category = "Jeunesse",
        Description = "L'amitié entre une jeune fille et un lion majestueux au Kenya.",
        PublishDate = new DateTime(1958, 1, 1),
        Publisher = "Gallimard",
        Language = "Français",
        Quantity = 4,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782070612826-L.jpg"
    },
    new Book
    {
        Title = "Croc-Blanc",
        Author = "Jack London",
        ISBN = "978-2253002864",
        Category = "Jeunesse",
        Description = "L'histoire poignante d'un chien-loup dans le Grand Nord.",
        PublishDate = new DateTime(1906, 5, 1),
        Publisher = "Le Livre de Poche",
        Language = "Français",
        Quantity = 5,
        CoverUrl = "https://covers.openlibrary.org/b/isbn/9782253002864-L.jpg"
    }
};

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            Console.WriteLine($"✓ {books.Length} livres créés avec succès !");
            Console.WriteLine($"📚 Catégories représentées: Classique, Roman, Science-Fiction, Fantasy, Policier, Horreur, Science, Informatique, BD, Manga, Philosophie, Art, Nature, Jeunesse");
        }

        /// <summary>
        /// ✅ MÉTHODE CORRIGÉE: Création des emprunts de test
        /// </summary>
        private static async Task CreateTestLoansAsync(LibraryDbContext context)
        {
            var random = new Random(42);

            // ✅ Charger les livres et abonnés AVEC tracking
            var books = await context.Books.ToListAsync();
            var subscribers = await context.Subscribers.ToListAsync();

            if (books.Count == 0 || subscribers.Count == 0)
            {
                Console.WriteLine("⚠ Impossible de créer des emprunts sans livres ou abonnés");
                return;
            }

            var loansCreated = 0;

            // ✅ EMPRUNTS ACTIFS (25)
            for (int i = 0; i < 35; i++)
            {
                var book = books[random.Next(books.Count)];
                var subscriber = subscribers[random.Next(subscribers.Count)];

                // ✅ Vérifier qu'il reste des exemplaires
                if (book.Quantity == 0)
                    continue;

                // ✅ Vérifier qu'il n'y a pas déjà un emprunt actif pour ce couple livre/abonné
                var existingLoan = await context.Loans
                    .AnyAsync(l => l.BookId == book.BookId
                                  && l.SubscriberId == subscriber.Id_User
                                  && l.IsActive);

                if (existingLoan)
                    continue;

                try
                {
                    var daysAgo = random.Next(1, 30);
                    var borrowDate = DateTime.Now.AddDays(-daysAgo);

                    // ✅ Calculer la date de retour avec bonus fidélité
                    int bonusDays = (int)(subscriber.Fidelity * 2);
                    var returnDate = borrowDate.AddDays(14 + bonusDays);

                    // ✅ Créer le prêt manuellement au lieu d'utiliser le constructeur
                    var loan = new Loan
                    {
                        BookId = book.BookId,
                        SubscriberId = subscriber.Id_User,
                        BorrowDate = borrowDate,
                        ReturnDate = returnDate,
                        IsActive = true,
                        Penalty = 0
                    };

                    // ✅ Décrémenter la quantité du livre
                    if (!book.TryDecreaseQuantity())
                    {
                        Console.WriteLine($"⚠ Impossible de décrémenter la quantité pour {book.Title}");
                        continue;
                    }

                    // ✅ Ajouter et sauvegarder
                    await context.Loans.AddAsync(loan);
                    await context.SaveChangesAsync(); // LoanId attribué

                    // ✅ Générer la référence
                    loan.GenerateReference();
                    await context.SaveChangesAsync(); // Référence persistée

                    loansCreated++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Erreur lors de la création d'un emprunt: {ex.Message}");
                    continue;
                }
            }

            // ✅ EMPRUNTS PASSÉS (20)
            for (int i = 0; i < 25; i++)
            {
                var book = books[random.Next(books.Count)];
                var subscriber = subscribers[random.Next(subscribers.Count)];

                try
                {
                    var daysAgo = random.Next(30, 90);
                    var borrowDate = DateTime.Now.AddDays(-daysAgo);

                    int bonusDays = (int)(subscriber.Fidelity * 2);
                    var returnDate = borrowDate.AddDays(14 + bonusDays);

                    var loan = new Loan
                    {
                        BookId = book.BookId,
                        SubscriberId = subscriber.Id_User,
                        BorrowDate = borrowDate,
                        ReturnDate = returnDate,
                        IsActive = false, // ✅ Déjà retourné
                        ActualReturnDate = returnDate.AddDays(random.Next(-3, 10)), // Retourné à temps ou en retard
                        Penalty = random.Next(0, 2) == 0 ? 0 : random.Next(5, 20)
                    };

                    await context.Loans.AddAsync(loan);
                    await context.SaveChangesAsync();

                    loan.GenerateReference();
                    await context.SaveChangesAsync();

                    loansCreated++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Erreur lors de la création d'un emprunt passé: {ex.Message}");
                    continue;
                }
            }

            // ✅ Compter les emprunts actifs et passés
            var activeCount = await context.Loans.CountAsync(l => l.IsActive);
            var inactiveCount = await context.Loans.CountAsync(l => !l.IsActive);

            Console.WriteLine($"✓ {loansCreated} emprunts créés ({activeCount} actifs, {inactiveCount} retournés)");
        }
    }
}
