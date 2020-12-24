using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;

namespace BookOnline.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db):base (db)
        {
            _db = db;
        }

        

      
    }
}
