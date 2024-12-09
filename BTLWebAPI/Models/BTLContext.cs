using Microsoft.EntityFrameworkCore;
using static BTLWebAPI.Models.Account;

namespace BTLWebAPI.Models
{
    public class BTLContext : DbContext
    {

        public BTLContext(DbContextOptions<BTLContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}