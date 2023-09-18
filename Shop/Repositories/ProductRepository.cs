using InternetShop.Data;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Repositories
{
	public class ProductRepository : IBaseRepository<Product>
    {
        private readonly ProductDbContext _db;
        public ProductRepository(ProductDbContext db)
        {
            _db = db;
        }
        public async Task Create(Product entity)
        {
            await _db.Products.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Product> GetAll()
        {
           return _db.Products;
        }
        public async Task Update(Product entity)
        {
            var oldEntity = await _db.FindAsync<Product>(entity.Id);
            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync();
        }
        public async Task Delete(Product entity)
        {
            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
        }

		public async Task DeleteAll(IQueryable<Product> entities)
		{
			_db.Products.RemoveRange(entities);
			await _db.SaveChangesAsync();
		}
	}
}
