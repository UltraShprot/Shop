using InternetShop.Data;
using Microsoft.EntityFrameworkCore;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Repositories
{
    public class CartRepository : IBaseRepository<CartProduct>
	{
		private readonly ProductDbContext _db;
		public CartRepository(ProductDbContext db)
		{
			_db = db;
		}
		public async Task Create(CartProduct entity)
		{
			await _db.CartProducts.AddAsync(entity);
			await _db.SaveChangesAsync();
		}

		public IQueryable<CartProduct> GetAll()
		{
			return _db.CartProducts;
		}
		public async Task Update(CartProduct entity)
		{
			var oldEntity = await _db.FindAsync<CartProduct>(entity.Id);
			_db.Entry(oldEntity).CurrentValues.SetValues(entity);
			await _db.SaveChangesAsync();
		}
		public async Task Delete(CartProduct entity)
		{
            _db.CartProducts.Remove(entity);
            await _db.SaveChangesAsync();
		}
		public async Task DeleteAll(IQueryable<CartProduct> entities)
		{
			_db.CartProducts.RemoveRange(entities);
			await _db.SaveChangesAsync();
		}
	}
}
