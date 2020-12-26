using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BookOnline.DataAccess;
using BookOnline.Models;
using BookOnline.DataAccess.Repository.IRepository;
using BookOnline.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace BookOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class User : Controller
    {
        private readonly ApplicationDbContext _db;

        public User(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            return View();
        }
        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in userList)
			{
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Reole = roles.FirstOrDefault(i => i.Id == roleId).Name;
				if (user.Company == null)
				{
                    user.Company = new Models.Company()
                    {
                        Name = ""
                    };
				}
			}
            return Json(new { data = userList });

        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
		{
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(i => i.Id == id);
			if (objFromDb == null)
			{
                return Json(new { success = false, message = "error while Locking/Unlocking." });

			}
            if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd>DateTime.Now)
			{
                //user is currently locked, we will unlock
                objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successfull." });
		}

       
        #endregion
    }
}
