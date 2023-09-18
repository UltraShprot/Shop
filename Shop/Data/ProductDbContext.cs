using Shop.Models;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using System.Xml;

namespace InternetShop.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
		public DbSet<CartProduct> CartProducts { get; set; }
		public DbSet<PurchasedProduct> PurchasedProducts { get; set; }

    }
}
