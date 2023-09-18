using InternetShop.Data;
using Shop.Data;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Repositories
{
	public class PurchasedProductRepository : IBaseRepository<PurchasedProduct>
	{
		private readonly ProductDbContext _db;
		public PurchasedProductRepository(ProductDbContext db)
		{
			_db = db;
		}
		public async Task Create(PurchasedProduct entity)
		{
			await _db.PurchasedProducts.AddAsync(entity);
			await _db.SaveChangesAsync();
		}

		public IQueryable<PurchasedProduct> GetAll()
		{
			return _db.PurchasedProducts;
		}
		public async Task Update(PurchasedProduct entity)
		{
			var oldEntity = await _db.FindAsync<PurchasedProduct>(entity.Id);
			_db.Entry(oldEntity).CurrentValues.SetValues(entity);
			await _db.SaveChangesAsync();
		}
		public async Task Delete(PurchasedProduct entity)
		{
			_db.PurchasedProducts.Remove(entity);
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAll(IQueryable<PurchasedProduct> entities)
		{
			_db.PurchasedProducts.RemoveRange(entities);
			await _db.SaveChangesAsync();
		}
	}
}
