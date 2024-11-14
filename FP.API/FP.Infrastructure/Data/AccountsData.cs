using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Infrastructure.Data
{
    public class AccountsData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = Guid.Parse("1096cb8b-4749-452d-aa08-a655d30ce2df"), Name = "Main", Balance = 0, IsDefault = true, Currency = "$" }
             );
        }
    }
}
