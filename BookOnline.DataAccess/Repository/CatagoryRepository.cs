using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using System.Linq;

namespace BookOnline.DataAccess.Repository
{
    public class CatagoryRepository : Repository<Catagory>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CatagoryRepository(ApplicationDbContext db):base (db)
        {
            _db = db;
        }

        

        public void Update(Catagory catagory)
        {
            var objFromDb = _db.Catagoires.FirstOrDefault(s => s.CatagoryId == catagory.CatagoryId);
            if (objFromDb != null)
            {
                objFromDb.Name = catagory.Name;

            }
        }
    }
}
