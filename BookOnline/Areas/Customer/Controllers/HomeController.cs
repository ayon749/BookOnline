using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using BookOnline.Models.ViewModels;
using BookOnline.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookOnline.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> ProductList = _unitOfWork.product.GetAll(includeProperties: "Catagory,CoverType");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (claims != null)
			{
                var count = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == claims.Value).ToList().Count();
               // HttpContext.Session.SetObject(SD.ssShoppingCart, count);
                 HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(ProductList);
        }
        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.product.GetFirstOrDefault(i => i.Id == id, includeProperties: "Catagory,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id

            };
            return View(cartObj);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
			if (ModelState.IsValid)
			{
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claims.Value;
                ShoppingCart cartFromDb = _unitOfWork.shoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId && u.ProductId == cartObject.ProductId
                    , includeProperties: "Product"
                    );

                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.shoppingCart.Add(cartObject);
                }
				else
				{
                    cartFromDb.Count += cartObject.Count;
                    _unitOfWork.shoppingCart.Update(cartFromDb);
				}
                _unitOfWork.Save();
                var count = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == cartObject.ApplicationUserId).ToList().Count();
               // HttpContext.Session.SetObject(SD.ssShoppingCart, count);
               HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
			else
			{
                var productFromDb = _unitOfWork.product.GetFirstOrDefault(i => i.Id == cartObject.ProductId, includeProperties: "Catagory,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id

                };
                return View(cartObj);
            }
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
