using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using System.Linq;

namespace BookOnline.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db):base (db)
        {
            _db = db;
        }



        public void Update(OrderDetails order)
        {
            _db.Update(order);
        }
    }
}
