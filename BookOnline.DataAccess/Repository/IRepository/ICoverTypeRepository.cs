using BookOnline.Models;

namespace BookOnline.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository :IRepository<CoverType>
    {
        public void Update(CoverType coverType);
    }
}
