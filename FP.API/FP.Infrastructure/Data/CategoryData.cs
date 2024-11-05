using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Infrastructure.Data
{
    public static class CategoryData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.NewGuid(), Name = "Needs", Color = "#FF5733" },
                new Category { Id = Guid.NewGuid(), Name = "Wants", Color = "#FFC300" },
                new Category { Id = Guid.NewGuid(), Name = "Savings", Color = "#28A745" },
                new Category { Id = Guid.NewGuid(), Name = "Subscriptions", Color = "#8E44AD" },
                new Category { Id = Guid.NewGuid(), Name = "Presents", Color = "#FF6347" },
                new Category { Id = Guid.NewGuid(), Name = "Salary", Color = "#3498DB" },
                new Category { Id = Guid.NewGuid(), Name = "Investments", Color = "#1ABC9C" },
                new Category { Id = Guid.NewGuid(), Name = "Loans", Color = "#E74C3C" },
                new Category { Id = Guid.NewGuid(), Name = "Bonuses", Color = "#F39C12" },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment", Color = "#9B59B6" },
                new Category { Id = Guid.NewGuid(), Name = "Freelance", Color = "#2ECC71" },
                new Category { Id = Guid.NewGuid(), Name = "Education", Color = "#2980B9" },
                new Category { Id = Guid.NewGuid(), Name = "Health", Color = "#C0392B" },
                new Category { Id = Guid.NewGuid(), Name = "Groceries", Color = "#27AE60" },
                new Category { Id = Guid.NewGuid(), Name = "Rent", Color = "#34495E" },
                new Category { Id = Guid.NewGuid(), Name = "Utilities", Color = "#BDC3C7" },
                new Category { Id = Guid.NewGuid(), Name = "Charity", Color = "#E67E22" },
                new Category { Id = Guid.NewGuid(), Name = "Interest", Color = "#8E44AD" },
                new Category { Id = Guid.NewGuid(), Name = "Side Hustle", Color = "#16A085" },
                new Category { Id = Guid.NewGuid(), Name = "Insurance", Color = "#D35400" }
            );
        }
    }
}
