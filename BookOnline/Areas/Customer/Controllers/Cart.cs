using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models;
using BookOnline.Models.ViewModels;
using BookOnline.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Twilio;

namespace BookOnline.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class Cart : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;
		[BindProperty]
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
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			shoppingCartVM = new ShoppingCartVM()
			{
				OrderHeader = new Models.OrderHeader(),
				ListCart = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == claim.Value,
															includeProperties: "Product")
			};

			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
															.GetFirstOrDefault(c => c.Id == claim.Value,
																includeProperties: "Company");

			foreach (var list in shoppingCartVM.ListCart)
			{
				list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
													list.Product.Price50, list.Product.Price100);
				shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
			}
			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
			shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
			shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			return View(shoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPost(string stripeToken)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
															.GetFirstOrDefault(c => c.Id == claim.Value,
																	includeProperties: "Company");

			shoppingCartVM.ListCart = _unitOfWork.shoppingCart
										.GetAll(c => c.ApplicationUserId == claim.Value,
										includeProperties: "Product");

			shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

			_unitOfWork.orderHeader.Add(shoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var item in shoppingCartVM.ListCart)
			{
				item.Price = SD.GetPriceBasedOnQuantity(item.Count, item.Product.Price,
					item.Product.Price50, item.Product.Price100);
				OrderDetails orderDetails = new OrderDetails()
				{
					ProductId = item.ProductId,
					OrderId = shoppingCartVM.OrderHeader.Id,
					Price = item.Price,
					Count = item.Count
				};
				shoppingCartVM.OrderHeader.OrderTotal += orderDetails.Count * orderDetails.Price;
				_unitOfWork.orderDetails.Add(orderDetails);

			}

			_unitOfWork.shoppingCart.RemoveRange(shoppingCartVM.ListCart);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);

			if (stripeToken == null)
			{
				//order will be created for delayed payment for authroized company
				shoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
			else
			{
				//process the payment
				var options = new ChargeCreateOptions
				{
					Amount = Convert.ToInt32(shoppingCartVM.OrderHeader.OrderTotal * 100),
					Currency = "usd",
					Description = "Order ID : " + shoppingCartVM.OrderHeader.Id,
					Source = stripeToken
				};

				var service = new ChargeService();
				Charge charge = service.Create(options);

				if (charge.Id == null)
				{
					shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
				}
				else
				{
					shoppingCartVM.OrderHeader.TransactionId = charge.Id;
				}
				if (charge.Status.ToLower() == "succeeded")
				{
					shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
					shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
					shoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
				}
			}

			_unitOfWork.Save();

			return RedirectToAction("OrderConfirmation", "Cart", new { id = shoppingCartVM.OrderHeader.Id });

		}
		public IActionResult OrderConfirmation(int id)
		{
			//OrderHeader orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == id);
			//TwilioClient.Init(_twilioOptions.AccountSid, _twilioOptions.AuthToken);
			//try
			//{
			//	var message = MessageResource.Create(
			//		body: "Order Placed on Bulky Book. Your Order ID:" + id,
			//		from: new Twilio.Types.PhoneNumber(_twilioOptions.PhoneNumber),
			//		to: new Twilio.Types.PhoneNumber(orderHeader.PhoneNumber)
			//		);
			//}
			//catch (Exception ex)
			//{

			//}



			return View(id);
		}
	}
}
