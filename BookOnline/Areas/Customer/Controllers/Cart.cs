using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models.ViewModels;
using BookOnline.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookOnline.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class Cart : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;

		public ShoppingCartVM shoppingCartVM { get; set; }
		public Cart(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
			_userManager = userManager;

		}
		
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartVM = new ShoppingCartVM() 
			{
				OrderHeader = new Models.OrderHeader(),
				ListCart = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"Product")
			};
			shoppingCartVM.OrderHeader.OrderTotal = 0;
			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
			foreach(var list in shoppingCartVM.ListCart)
			{
				list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);

				shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
				list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
				if (list.Product.Description.Length > 100)
				{
					list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
				}
			}
			return View(shoppingCartVM);
		}
		public IActionResult Plus(int CartId)
		{
			var cart = _unitOfWork.shoppingCart.GetFirstOrDefault(c => c.Id == CartId, includeProperties: "Product");
			cart.Count += 1;
			cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.shoppingCart.GetFirstOrDefault
							(c => c.Id == cartId, includeProperties: "Product");

			if (cart.Count == 1)
			{
				var cnt = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
				_unitOfWork.shoppingCart.Remove(cart);
				_unitOfWork.Save();
				HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
			}
			else
			{
				cart.Count -= 1;
				cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
									cart.Product.Price50, cart.Product.Price100);
				_unitOfWork.Save();
			}

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.shoppingCart.GetFirstOrDefault
							(c => c.Id == cartId, includeProperties: "Product");

			var cnt = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
			_unitOfWork.shoppingCart.Remove(cart);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);


			return RedirectToAction(nameof(Index));
		}
	}
}
