using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIBBRARY_MANAGER.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PublishDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsRemoved = table.Column<bool>(type: "INTEGER", nullable: false),
                    CoverUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id_User = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name_User = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Password_Hash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Adresse_Mail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Num_Telephone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Date_Creation = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id_User);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id_User = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Poste = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    YearHired = table.Column<int>(type: "INTEGER", nullable: false),
                    Ref_Staff = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id_User);
                    table.ForeignKey(
                        name: "FK_StaffMembers_Users_Id_User",
                        column: x => x.Id_User,
                        principalTable: "Users",
                        principalColumn: "Id_User",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id_User = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fidelity = table.Column<decimal>(type: "TEXT", precision: 4, scale: 2, nullable: false),
                    Ref_Subscriber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id_User);
                    table.ForeignKey(
                        name: "FK_Subscribers_Users_Id_User",
                        column: x => x.Id_User,
                        principalTable: "Users",
                        principalColumn: "Id_User",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubscriberId = table.Column<long>(type: "INTEGER", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Penalty = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Ref_Loan = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "Id_User",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modifications",
                columns: table => new
                {
                    ModificationId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StaffMemberId = table.Column<long>(type: "INTEGER", nullable: false),
                    LoanId = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Ref_Modification = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OldReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NewReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifications", x => x.ModificationId);
                    table.ForeignKey(
                        name: "FK_Modifications_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Modifications_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id_User",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_Ref_Loan",
                table: "Loans",
                column: "Ref_Loan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_SubscriberId",
                table: "Loans",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_Modifications_LoanId",
                table: "Modifications",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Modifications_Ref_Modification",
                table: "Modifications",
                column: "Ref_Modification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modifications_StaffMemberId",
                table: "Modifications",
                column: "StaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_Ref_Staff",
                table: "StaffMembers",
                column: "Ref_Staff",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_Ref_Subscriber",
                table: "Subscribers",
                column: "Ref_Subscriber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Adresse_Mail",
                table: "Users",
                column: "Adresse_Mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modifications");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
