using BookOnline.Models;

namespace BookOnline.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Catagory>
    {
        public void Update(Catagory catagory);
    }
}
