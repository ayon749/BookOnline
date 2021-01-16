using System;

namespace BookOnline.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork :IDisposable
    {
        ICategoryRepository Catagory { get; }
        ICoverTypeRepository coverType { get; }

        IProductRepository product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository shoppingCart { get; }

        IOrderHeaderRepository orderHeader { get; }
        IOrderDetailsRepository orderDetails { get; }

        IApplicationUserRepository ApplicationUser { get; }
        ISP_Call SP_Call { get; }

         void Save();
    }
}
