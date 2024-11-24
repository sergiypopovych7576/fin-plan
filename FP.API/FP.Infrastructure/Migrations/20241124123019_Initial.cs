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
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    IconName = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "DATE", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "DATE", nullable: true),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetAccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledOperations_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledOperations_Accounts_TargetAccountId",
                        column: x => x.TargetAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledOperations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Applied = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    ScheduledOperationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetAccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_Accounts_TargetAccountId",
                        column: x => x.TargetAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Operations_ScheduledOperations_ScheduledOperationId",
                        column: x => x.ScheduledOperationId,
                        principalTable: "ScheduledOperations",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "Currency", "IsDefault", "Name" },
                values: new object[] { new Guid("1096cb8b-4749-452d-aa08-a655d30ce2df"), 0m, "$", true, "Main" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "IconName", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("11fd6637-e7c1-42b1-a930-54f1781c6335"), "#16A085", "pets", "Pets", 1 },
                    { new Guid("1aedf196-748a-47d5-8f98-3e605d56ca52"), "#FFA500", "house", "Rent", 1 },
                    { new Guid("1c258957-8e7a-4ed3-9594-5f5e3d9cea73"), "#F1C40F", "call", "Communication", 1 },
                    { new Guid("1f950a9b-ca55-4dc1-b2d1-a443ff406216"), "#FF6347", "redeem", "Presents", 1 },
                    { new Guid("391b742e-b375-4f90-b1e2-c367dc81ad45"), "#EE4B2B", "payments", "Bills", 1 },
                    { new Guid("39614ffb-5c28-47fb-8ef8-d00955318000"), "#D35400", "medical_information", "Insurance", 1 },
                    { new Guid("3edb33ba-29f5-485b-ac98-b2755216644f"), "#8E44AD", "fitness_center", "Hobby", 1 },
                    { new Guid("3f5b9dd5-7ab3-4872-947a-e24955a0b53d"), "#5D6D7E", "subscriptions", "Subscriptions", 1 },
                    { new Guid("4bd0535c-65ec-458a-af6b-bdf6e2112766"), "#2ECC71", "paid", "Freelance", 0 },
                    { new Guid("53610993-b452-4aa1-95a4-528bd4e3964c"), "#FFC300", "paid", "Bonuses", 0 },
                    { new Guid("6b5d325d-fd01-41cb-8cfb-6f8edec25867"), "#ffd014", "shopping_cart", "Purchases", 1 },
                    { new Guid("6cdd74dc-e30e-4eda-a738-6dfb0c3c2e47"), "#E74C3C", "paid", "Loans", 1 },
                    { new Guid("6f5cd858-8065-40c1-938a-1d8213bd8c9c"), "#228B22", "paid", "Investments", 0 },
                    { new Guid("975dcdee-463e-45ab-a9c7-b61a5db39746"), "#3366FF", "paid", "Salary", 0 },
                    { new Guid("9fccf2dc-300f-4eea-ae49-94f990b696b9"), "#9B59B6", "sports_esports", "Entertainment", 1 },
                    { new Guid("a2f0cc9b-ce0e-4134-861b-c9e2af759ff0"), "#8E44AD", "savings", "Interest", 0 },
                    { new Guid("b10d0f9b-45d5-4913-9406-ca4e314f67ff"), "#0fa30f", "people", "Family", 1 },
                    { new Guid("b51b9690-1c03-46c0-aa3f-d5113ca5b450"), "#C0392B", "medical_information", "Health", 1 },
                    { new Guid("b95c2a2c-800d-4e9b-bf02-309cfef03de1"), "#3498DB", "local_taxi", "Taxi", 1 },
                    { new Guid("c42eedc8-b1d8-44fd-ae0d-834dc523de24"), "#1ABC9C", "drive_eta", "Transport", 1 },
                    { new Guid("e5e9f603-5f0c-4c57-93f0-d7ce03e1b8d0"), "#2980B9", "school", "Education", 1 },
                    { new Guid("f1d4e014-a973-4a3e-a421-2b955e11d33f"), "#8E44AD", "trending_flat", "Transfer", 2 },
                    { new Guid("f552e38a-642f-4e61-9ab6-0b4f64320e43"), "#F39C12", "volunteer_activism", "Charity", 1 },
                    { new Guid("f6f14b4d-5418-4f40-9909-896428fa6d86"), "#E74C3C", "restaurant", "Restaurants", 1 },
                    { new Guid("fd522056-8512-4a5c-9a87-6f1d53193af7"), "#27AE60", "local_grocery_store", "Groceries", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CategoryId",
                table: "Operations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ScheduledOperationId",
                table: "Operations",
                column: "ScheduledOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_SourceAccountId",
                table: "Operations",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_TargetAccountId",
                table: "Operations",
                column: "TargetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledOperations_CategoryId",
                table: "ScheduledOperations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledOperations_SourceAccountId",
                table: "ScheduledOperations",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledOperations_TargetAccountId",
                table: "ScheduledOperations",
                column: "TargetAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "ScheduledOperations");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
