using BookOnline.DataAccess.Repository;
using BookOnline.DataAccess.Data;
using BookOnline.DataAccess.Repository.IRepository;

namespace BookOnline.DataAccess.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Catagory = new CatagoryRepository(_db);
            coverType = new CoverTypeRepository(_db);
            product = new ProductRepository(_db);
            SP_Call = new SP_Call(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            orderHeader = new OrderHeaderRepository(_db);
            orderDetails = new OrderDetailsRepository(_db);
        }
        public ICategoryRepository Catagory { get; private set; }

        

        public ISP_Call SP_Call { get; private set; }

        public ICoverTypeRepository coverType { get; private set; }
       // public IProductRepository Product { get; private set; }

        public IProductRepository product { get; private set; }

        public ICompanyRepository Company { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }

		//public IShoppingCartRepository shoppingCart => throw new System.NotImplementedException();

		public IOrderHeaderRepository orderHeader { get; private set; }

		//public IOrderHeaderRepository orderHeader => throw new System.NotImplementedException();

		public IOrderDetailsRepository orderDetails { get; private set; }

		//public IOrderDetailsRepository orderDetails => throw new System.NotImplementedException();

		public void Dispose()
        {
            _db.Dispose();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
