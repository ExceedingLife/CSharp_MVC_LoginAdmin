using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_LoginAdmin.Controllers;
using MVC_LoginAdmin.Models;

namespace MVC_LoginAdmin.Models
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context;
        ApplicationUser appuser;

        public AdminController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult AdminManage()
        {
            var roleList = context.Roles.OrderBy(r => r.Name).ToList()
                .Select(rr => new SelectListItem { Value = rr.Name.ToString(),
                Text = rr.Name }).ToList();
            ViewBag.Roles = roleList;

            var userList = context.Users.OrderBy(u => u.UserName).ToList()
                .Select(uu => new SelectListItem { Value = uu.UserName.ToString(),
                Text = uu.UserName }).ToList();
            ViewBag.Users = userList;

            ViewBag.Message = "";

            return View();
        }

        // GET: /Roles/Create
        public ActionResult CreateRole()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public ActionResult CreateRole(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new IdentityRole(){
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.Message = "Role Created Successfully !";
                return RedirectToAction("AdminManage");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Roles/Edit/5
        public ActionResult EditRole(string roleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return View(thisRole);
        }
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("AdminManage");
            }
            catch
            {
                return View();
            }
        }

        // Add role to user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string username, string rolename)
        {
            if (context == null)
                throw new ArgumentNullException("context", "Contect cant be NULL.");
            ApplicationUser au = context.Users.Where(u => u.UserName.Equals(username,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            manager.AddToRole(au.Id, rolename);
            ViewBag.Message = "Role Created Successfully!";
            // REPOPULATE DROPDOWN LISTS
            var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
              new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = roleList;
            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
              new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userList;

            return View("AdminManage");
        }

        // Get Roles for a User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRolesForUser(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                ApplicationUser au = context.Users.Where(u => u.UserName.Equals(username,
                    StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                ViewBag.RolesForUser = manager.GetRoles(au.Id);
                // REPOPULATE DROPDOWN LISTS
                var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                  new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = roleList;
                var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                  new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                ViewBag.Users = userList;
                ViewBag.Message = "Roles retrieved successfully !";
            }
            return View("AdminManage");
        }

        // Delete Role for a User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string username, string rolename)
        {
            var account = new AccountController();
            ApplicationUser au = context.Users.Where(u => u.UserName.Equals(username,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            if(manager.IsInRole(au.Id, rolename))
            {
                manager.RemoveFromRole(au.Id, rolename);
                ViewBag.Message = "Role removed successful for user !";
            }
            else
            {
                ViewBag.Message = "This user doesn't have this ROLE !";
            }
            // REPOPULATE DROPDOWN LISTS
            var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
              new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = roleList;
            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
              new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userList;

            return View("AdminManage");
        }

        // View (CAN CREATE ROLE)
        public ActionResult ViewCreateRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!IsAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var roles = context.Roles.ToList();
            return View(roles);
        }

        // View (ACTUAL CREATE ROLE)
        public ActionResult CreateTheRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!IsAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            var role = new IdentityRole();
            return View(role);
        }
        [HttpPost]
        public ActionResult CreateTheRole(IdentityRole role)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!IsAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            context.Roles.Add(role);
            context.SaveChanges();
            TempData["Success"] = "New Role Successfully Created.";
            return RedirectToAction("ViewCreateRole", "Users");
        }

        // DELETE ROLE
        public ActionResult DeleteRole(string rolename)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(rolename,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            TempData["Error"] = "Role Successfully Deleted.";
            //return RedirectToAction("ViewCreateRole", "Users");
            return RedirectToAction("AdminManage", "Admin");
        }

        public bool IsAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var result = User.IsInRole("Admin");

                if (result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}