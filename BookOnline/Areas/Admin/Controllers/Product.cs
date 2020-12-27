using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using BookOnline.Utility;

namespace BookOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class Product : Controller
    {
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IWebHostEnvironment _iWebHostEnvironment;
        
        public Product(IUnitOfWork iUnitOfWork, IWebHostEnvironment iWebHostEnvironment)
        {
            _iUnitOfWork = iUnitOfWork;
            _iWebHostEnvironment = iWebHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVm = new ProductVM()
            {
                Product = new BookOnline.Models.Product(),
                CatagoryList = _iUnitOfWork.Catagory.GetAll().Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.CatagoryId.ToString()
                }),
                CoverList = _iUnitOfWork.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CoverId.ToString()
                })

            };
            if (id == null)
            {
                return View(productVm);
            }
            productVm.Product = _iUnitOfWork.product.Get(id.GetValueOrDefault());
            if (productVm.Product == null)
            {
                return NotFound();
            }
            return View(productVm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productvm)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _iWebHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (productvm.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productvm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) 
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    productvm.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    if (productvm.Product.Id != 0)
                    {
                        BookOnline.Models.Product productFromDb = _iUnitOfWork.product.Get(productvm.Product.Id);
                        productvm.Product.ImageUrl = productFromDb.ImageUrl;
                    }
                }

                if (productvm.Product.Id == 0)
                {
                    _iUnitOfWork.product.Add(productvm.Product);
                }
                else
                {
                    _iUnitOfWork.product.Update(productvm.Product);
                }
                _iUnitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<BookOnline.Models.Catagory> CatList = _iUnitOfWork.Catagory.GetAll();
                productvm.CatagoryList = CatList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CatagoryId.ToString()
                }) ;
                productvm.CoverList = _iUnitOfWork.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CoverId.ToString()
                });
                if (productvm.Product.Id != 0)
                {
                    productvm.Product = _iUnitOfWork.product.Get(productvm.Product.Id);
                }
            }
            return View(productvm);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _iUnitOfWork.product.GetAll(includeProperties:"Catagory,CoverType");

            return Json(new { data = allObj });

        }

        [HttpDelete]

        public IActionResult Delete(int id)
        {
            var catFromDb = _iUnitOfWork.product.Get(id);
            if (catFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _iWebHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, catFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _iUnitOfWork.product.Remove(catFromDb);
            _iUnitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
