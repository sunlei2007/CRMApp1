using CRMAppEntity;
using Microsoft.EntityFrameworkCore;
using System;
namespace CRMApp
{
    public class DBContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-R0ORLLEH;Initial Catalog=Order;User ID=sa;Password=111111");
            optionsBuilder.LogTo(Console.WriteLine);
        }

        public virtual DbSet<Customer> CustomerSet { get; set; }
        public virtual DbSet<CreditCard> CreditCardSet { get; set; }
        public virtual DbSet<User> UserSet { get; set; }
    }
   
}
