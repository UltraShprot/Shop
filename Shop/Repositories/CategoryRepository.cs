using InternetShop.Data;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Repositories
{
	public class CategoryRepository : IBaseRepository<Category>
	{
		private readonly ProductDbContext _db;
		public CategoryRepository(ProductDbContext db)
		{
			_db = db;
		}
		public async Task Create(Category entity)
		{
			await _db.Categories.AddAsync(entity);
			await _db.SaveChangesAsync();
		}

		public IQueryable<Category> GetAll()
		{
			return _db.Categories;
		}
		public async Task Update(Category entity)
		{
			var oldEntity = await _db.FindAsync<Category>(entity.Id);
			_db.Entry(oldEntity).CurrentValues.SetValues(entity);
			await _db.SaveChangesAsync();
		}
		public async Task Delete(Category entity)
		{
			_db.Categories.Remove(entity);
			await _db.SaveChangesAsync();
		}
		public async Task DeleteAll(IQueryable<Category> entities)
		{
			_db.Categories.RemoveRange(entities);
			await _db.SaveChangesAsync();
		}
	}
}
