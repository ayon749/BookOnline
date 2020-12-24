using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using System.Linq;

namespace BookOnline.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(CoverType coverType)
        {
            var CoverTypeFromDb = _db.Covers.FirstOrDefault(s => s.CoverId == coverType.CoverId);
            if (CoverTypeFromDb != null)
            {
                CoverTypeFromDb.Name = coverType.Name;
                _db.SaveChanges();
            }
        }
    }
}
