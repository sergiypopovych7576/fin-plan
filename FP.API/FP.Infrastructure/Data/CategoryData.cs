using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Infrastructure.Data
{
    public static class CategoryData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("f1d4e014-a973-4a3e-a421-2b955e11d33f"), Name = "Transfer", Color = "#8E44AD", IconName = "trending_flat", Type = OperationType.Transfer },

                new Category { Id = Guid.Parse("975dcdee-463e-45ab-a9c7-b61a5db39746"), Name = "Salary", Color = "#3366FF", IconName = "paid", Type = OperationType.Income },
                new Category { Id = Guid.Parse("6f5cd858-8065-40c1-938a-1d8213bd8c9c"), Name = "Investments", Color = "#228B22", IconName = "paid", Type = OperationType.Income },
                new Category { Id = Guid.Parse("53610993-b452-4aa1-95a4-528bd4e3964c"), Name = "Bonuses", Color = "#FFC300", IconName = "paid", Type = OperationType.Income },
                new Category { Id = Guid.Parse("4bd0535c-65ec-458a-af6b-bdf6e2112766"), Name = "Freelance", Color = "#2ECC71", IconName = "paid", Type = OperationType.Income },
                new Category { Id = Guid.Parse("a2f0cc9b-ce0e-4134-861b-c9e2af759ff0"), Name = "Interest", Color = "#8E44AD", IconName = "savings", Type = OperationType.Income },

                new Category { Id = Guid.Parse("1aedf196-748a-47d5-8f98-3e605d56ca52"), Name = "Rent", Color = "#FFA500", IconName = "house", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("391b742e-b375-4f90-b1e2-c367dc81ad45"), Name = "Bills", Color = "#EE4B2B", IconName = "payments", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("6cdd74dc-e30e-4eda-a738-6dfb0c3c2e47"), Name = "Loans", Color = "#E74C3C", IconName = "paid", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("3f5b9dd5-7ab3-4872-947a-e24955a0b53d"), Name = "Subscriptions", Color = "#5D6D7E", IconName = "subscriptions", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("39614ffb-5c28-47fb-8ef8-d00955318000"), Name = "Insurance", Color = "#D35400", IconName = "medical_information", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("fd522056-8512-4a5c-9a87-6f1d53193af7"), Name = "Groceries", Color = "#27AE60", IconName = "local_grocery_store", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("b51b9690-1c03-46c0-aa3f-d5113ca5b450"), Name = "Health", Color = "#C0392B", IconName = "medical_information", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("e5e9f603-5f0c-4c57-93f0-d7ce03e1b8d0"), Name = "Education", Color = "#2980B9", IconName = "school", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("1f950a9b-ca55-4dc1-b2d1-a443ff406216"), Name = "Presents", Color = "#FF6347", IconName = "redeem", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("9fccf2dc-300f-4eea-ae49-94f990b696b9"), Name = "Entertainment", Color = "#9B59B6", IconName = "sports_esports", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("c42eedc8-b1d8-44fd-ae0d-834dc523de24"), Name = "Transport", Color = "#1ABC9C", IconName = "drive_eta", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("f6f14b4d-5418-4f40-9909-896428fa6d86"), Name = "Restaurants", Color = "#E74C3C", IconName = "restaurant", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("3edb33ba-29f5-485b-ac98-b2755216644f"), Name = "Hobby", Color = "#8E44AD", IconName = "fitness_center", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("b95c2a2c-800d-4e9b-bf02-309cfef03de1"), Name = "Taxi", Color = "#3498DB", IconName = "local_taxi", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("1c258957-8e7a-4ed3-9594-5f5e3d9cea73"), Name = "Communication", Color = "#F1C40F", IconName = "call", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("11fd6637-e7c1-42b1-a930-54f1781c6335"), Name = "Pets", Color = "#16A085", IconName = "pets", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("f552e38a-642f-4e61-9ab6-0b4f64320e43"), Name = "Charity", Color = "#F39C12", IconName = "volunteer_activism", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("6b5d325d-fd01-41cb-8cfb-6f8edec25867"), Name = "Purchases", Color = "#ffd014", IconName = "shopping_cart", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("b10d0f9b-45d5-4913-9406-ca4e314f67ff"), Name = "Family", Color = "#0fa30f", IconName = "people", Type = OperationType.Expense },
                new Category { Id = Guid.Parse("4c8fd579-e5ef-4c58-baf1-23712978615c"), Name = "Clothes", Color = "#1684c4", IconName = "checkroom", Type = OperationType.Expense }
             );
        }
    }
}
