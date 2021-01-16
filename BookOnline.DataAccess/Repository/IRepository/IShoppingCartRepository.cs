using BookOnline.Models;

namespace BookOnline.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        public void Update(ShoppingCart shoppingCart);
    }
}
