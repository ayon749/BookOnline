using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using System.Linq;

namespace BookOnline.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db):base (db)
        {
            _db = db;
        }



        public void Update(OrderHeader order)
        {
            _db.Update(order);
        }
    }
}
