using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Color = table.Column<string>(type: "text", nullable: false)
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
                columns: new[] { "Id", "Color", "Name" },
                values: new object[,]
                {
                    { new Guid("18bdd321-c65c-4bc4-96f8-15883d95fb4a"), "#2ECC71", "Freelance" },
                    { new Guid("33db6d81-7c07-44a1-bdc8-5684352242fe"), "#BDC3C7", "Utilities" },
                    { new Guid("3d5fac86-11e6-4aed-b5d5-ea5d399e7837"), "#F39C12", "Bonuses" },
                    { new Guid("417cfc76-4454-4833-b493-3bca9a62ca97"), "#2980B9", "Education" },
                    { new Guid("4f330be9-41b5-4229-871b-36853f480b8b"), "#3498DB", "Salary" },
                    { new Guid("4f46f8e0-86c3-4b77-acad-60d51c9228d6"), "#D35400", "Insurance" },
                    { new Guid("6af58cb8-81e6-4d4c-95e2-81cb9da2a128"), "#8E44AD", "Subscriptions" },
                    { new Guid("7265a21c-80b8-4e12-9e9e-860e4c554918"), "#E74C3C", "Loans" },
                    { new Guid("77b95a66-8796-4363-93a5-8ce752a238d7"), "#27AE60", "Groceries" },
                    { new Guid("8f0eab38-e186-4801-a2af-14e56494382f"), "#8E44AD", "Interest" },
                    { new Guid("96fbede3-2676-4889-bbc3-e7e018bae1ac"), "#28A745", "Savings" },
                    { new Guid("a0c5ebc9-a9bb-4317-9251-9dfa717dc8fb"), "#16A085", "Side Hustle" },
                    { new Guid("bba28a72-f54e-4ad0-8e2c-7b144e08bc1d"), "#1ABC9C", "Investments" },
                    { new Guid("cb4cbf22-e728-487e-b6e1-f01dda96b8a6"), "#FF6347", "Presents" },
                    { new Guid("d1b12a7b-9985-40da-b84d-f81c8a3b161b"), "#9B59B6", "Entertainment" },
                    { new Guid("daa73fdd-22ee-4d93-9ebb-f2953633df44"), "#C0392B", "Health" },
                    { new Guid("e53e33d1-afcf-4608-a0cf-c667da634cbc"), "#34495E", "Rent" },
                    { new Guid("e7d28a89-80e4-4071-866e-e97ab2ef29b8"), "#E67E22", "Charity" },
                    { new Guid("e91d46fc-3b57-49d1-b9ea-d103daec6506"), "#FFC300", "Wants" },
                    { new Guid("ea0cd471-45a6-43fc-959f-a79a862dd1ac"), "#FF5733", "Needs" }
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
