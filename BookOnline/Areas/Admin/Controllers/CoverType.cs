using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BookOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverType : Controller
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public CoverType(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            BookOnline.Models.CoverType cover = new BookOnline.Models.CoverType();
            if (id == null)
            {
                return View(cover);
            }
            var parameter = new DynamicParameters();
            parameter.Add("@CoverId", id);
           cover= _iUnitOfWork.SP_Call.OneRecord<Models.CoverType>(SD.Proc_CoverType_Get, parameter);
            if (cover == null)
            {
                return NotFound();
            }
            return View(cover);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BookOnline.Models.CoverType cover)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Name", cover.Name);
            if (ModelState.IsValid)
            {
                if (cover.CoverId == 0)
                {
                    _iUnitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                }
                else
                {
                    parameter.Add("@CoverId", cover.CoverId);
                    _iUnitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }
                _iUnitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(cover);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _iUnitOfWork.SP_Call.List<BookOnline.Models.CoverType>(SD.Proc_CoverType_GetAll, null);

            return Json(new { data = allObj });

        }
        [HttpDelete]

        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@CoverId", id);
            var coverFromDb = _iUnitOfWork.SP_Call.OneRecord<Models.CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _iUnitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _iUnitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
