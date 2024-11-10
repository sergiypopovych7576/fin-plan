using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("1dbbf7b0-c06e-4c5a-99fe-825010996a1b"), "#34495E", "Rent", 1 },
                    { new Guid("367116cf-fa31-4754-b989-8309c0d745a8"), "#E67E22", "Charity", 1 },
                    { new Guid("3b7df4a1-9d8b-48a2-abd7-3cbaf1631d67"), "#BDC3C7", "Utilities", 1 },
                    { new Guid("3e57aa1c-6763-4ad4-8093-1880069e914b"), "#E74C3C", "Loans", 0 },
                    { new Guid("3f1c9129-9a5e-4b04-b929-3e3dc2aca725"), "#8E44AD", "Subscriptions", 1 },
                    { new Guid("401f1c6d-9765-4341-81c7-87e16d09e93e"), "#1ABC9C", "Investments", 0 },
                    { new Guid("4c3397c9-f22a-413d-b4f7-b45aa0369350"), "#F39C12", "Bonuses", 0 },
                    { new Guid("4ed9c859-14c8-4866-a256-fd2b0aaa68e4"), "#FF6347", "Presents", 1 },
                    { new Guid("79de3e51-3b72-48c1-9eb1-c48c7cdce746"), "#FFC300", "Wants", 1 },
                    { new Guid("862442fc-2473-47ab-8b92-9182f73f1bbd"), "#D35400", "Insurance", 1 },
                    { new Guid("9fb465b5-3097-4641-befe-43e8916a9d3b"), "#3498DB", "Salary", 0 },
                    { new Guid("a32456f2-e734-47a0-b736-b734bd655226"), "#28A745", "Savings", 1 },
                    { new Guid("aa962b69-f3b7-4e43-bd96-093bbc0b71e9"), "#8E44AD", "Interest", 0 },
                    { new Guid("ab441d80-a897-4cd5-b50c-52fffda543d2"), "#16A085", "Side Hustle", 0 },
                    { new Guid("addc0b03-b632-4c50-98f4-94871bef630e"), "#9B59B6", "Entertainment", 1 },
                    { new Guid("ce9f563c-a479-419c-a1e9-15cf38f00209"), "#27AE60", "Groceries", 1 },
                    { new Guid("dcdbc0c0-6399-4dbe-bfc6-39351b86763d"), "#FF5733", "Needs", 1 },
                    { new Guid("f4743f89-f9da-4697-999b-45147423cce3"), "#2980B9", "Education", 1 },
                    { new Guid("f536adb6-c623-43c4-abfb-635568ce941b"), "#C0392B", "Health", 1 },
                    { new Guid("f6b969f9-5d39-4d46-a021-d17523980ce6"), "#2ECC71", "Freelance", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CategoryId",
                table: "Operations",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
