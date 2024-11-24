using FP.Domain;
using FP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FP.Infrastructure
{
    public class BudgetDbContext : DbContext
    {
        public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<ScheduledOperation> ScheduledOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CategoryData.Seed(modelBuilder);
            AccountsData.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Operation>()
                .Property(o => o.Date)
                .HasColumnType("DATE");
            modelBuilder.Entity<ScheduledOperation>()
                .Property(o => o.StartDate)
                .HasColumnType("DATE");
            modelBuilder.Entity<ScheduledOperation>()
               .Property(o => o.EndDate)
               .HasColumnType("DATE");
            modelBuilder.Entity<Operation>().HasOne(o => o.SourceAccount)
          .WithMany() // If no navigation property in Account, keep this; otherwise, specify it
          .HasForeignKey(o => o.SourceAccountId)
          .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes if SourceAccount is deleted
            modelBuilder.Entity<Operation>().HasOne(o => o.TargetAccount)
                .WithMany() // If no navigation property in Account, keep this; otherwise, specify it
                .HasForeignKey(o => o.TargetAccountId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes if TargetAccount is deleted
        }
    }
}
