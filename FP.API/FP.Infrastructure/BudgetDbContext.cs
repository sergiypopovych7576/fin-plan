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
        }
    }
}
