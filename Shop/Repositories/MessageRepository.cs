using Shop.Data;
using Shop.Interfaces;
using Shop.Models;

namespace Shop.Repositories
{
    public class MessageRepository: IBaseRepository<Message>
	{
        private readonly ApplicationDbContext _db;
        public MessageRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task Create(Message entity)
        {
            await _db.Messages.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Message> GetAll()
        {
            return _db.Messages;
        }
        public async Task Update(Message entity)
        {
            var oldEntity = await _db.FindAsync<CartProduct>(entity.Id);
            _db.Entry(oldEntity).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync();
        }
        public async Task Delete(Message entity)
        {
            _db.Messages.Remove(entity);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAll(IQueryable<Message> entities)
        {
            _db.Messages.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }
    }
}
