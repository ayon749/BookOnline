using BookOnline.Models;

namespace BookOnline.DataAccess.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        public void Update(OrderDetails orderDetails);
    }
}
