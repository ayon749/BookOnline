using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using System.Linq;

namespace BookOnline.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db):base (db)
        {
            _db = db;
        }

        

        public void Update(ShoppingCart shopping)
        {
            _db.Update(shopping);
        }
    }
}
