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
                new Category { Id = Guid.NewGuid(), Name = "Salary", Color = "#005CBB", Type = OperationType.Income },
                new Category { Id = Guid.NewGuid(), Name = "Investments", Color = "#228B22", Type = OperationType.Income },
                new Category { Id = Guid.NewGuid(), Name = "Loans", Color = "#E74C3C", Type = OperationType.Income },
                new Category { Id = Guid.NewGuid(), Name = "Bonuses", Color = "#FFC300", Type = OperationType.Income },
                new Category { Id = Guid.NewGuid(), Name = "Freelance", Color = "#2ECC71", Type = OperationType.Income },
                new Category { Id = Guid.NewGuid(), Name = "Interest", Color = "#8E44AD", Type = OperationType.Income },

                new Category { Id = Guid.NewGuid(), Name = "Rent", Color = "#FFA500", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Subscriptions", Color = "#5D6D7E", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Insurance", Color = "#D35400", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Groceries", Color = "#27AE60", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Health", Color = "#C0392B", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Education", Color = "#2980B9", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Presents", Color = "#FF6347", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment", Color = "#9B59B6", Type = OperationType.Expense },
                new Category { Id = Guid.NewGuid(), Name = "Charity", Color = "#F39C12", Type = OperationType.Expense }
             );
        }
    }
}
