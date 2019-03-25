using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Data.Entity;
using MVC_LoginAdmin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;


namespace MVC_LoginAdmin.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        ApplicationDbContext context;
        ApplicationUser appuser;

        public UsersController()
        {
            context = new ApplicationDbContext();
            //appuser = System.Web.HttpContext.Current.GetOwinContext()
            //    .GetUserManager<ApplicationUserManager>().FindById(
            //    System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        // User Home Page - SIGNED IN
        // GET: Users
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.displayMenu = "No";
                if (IsAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged In";
            }
            return View();
        }

        // IS USER ADMINISTRATOR ??
        //public bool IsAdminUser()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var user = User.Identity;
        //        var manager = new UserManager<ApplicationUser>
        //            (new UserStore<ApplicationUser>(context));
        //        var role = UserManager.GetRoles(user.GetUserId());
        //        if (role[0].ToString() == "Admin")
        //            return true;
        //        else
        //            return false;
        //    }
        //    return false;
        //}
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
            return RedirectToAction("ViewCreateRole", "Users");
        }

        // VIEW ALL USERS + ROLES
        [AllowAnonymous]
        public ActionResult UsersWithRoles()
        {
            var sql = @"SELECT u.id AS UserId, u.Email, u.Username, r.Name AS Role
                            FROM [dbo].[AspNetUsers] u
                            LEFT JOIN [dbo].[AspNetUserRoles] ar
                            ON ar.UserId = u.Id 
                            LEFT JOIN [dbo].[AspNetRoles] r 
                            ON r.Id = ar.RoleId";
            var result = context.Database.SqlQuery<UserWithRole>(sql).ToList();
            return View(result);
        }

        // View Selected User
        public ActionResult Details(string username)
        {
            if (username == null)
                    return HttpNotFound();

            ApplicationUser au = context.Users.First(u => u.UserName == username);
            UpdateViewModel upmodel = new UpdateViewModel();
            if(au.Roles.Count == 0)
            {
                upmodel.UserId = au.Id;
                upmodel.Firstname = au.FirstName;
                upmodel.Lastname = au.LastName;
                upmodel.PhoneNumber = au.PhoneNumber;
                upmodel.Email = au.Email;
                upmodel.UserName = au.UserName;
                return View(upmodel);
            }
            else
            { 
                foreach(IdentityUserRole role in au.Roles)
                {
                    if(role != null)  
                    {
                        string rid = role.RoleId;
                        string roleName = context.Roles.First(r => r.Id == role.RoleId).Name;
                        upmodel.UserId = au.Id;
                        upmodel.Firstname = au.FirstName;
                        upmodel.Lastname = au.LastName;
                        upmodel.PhoneNumber = au.PhoneNumber;
                        upmodel.Email = au.Email;
                        upmodel.UserName = au.UserName;
                        upmodel.UserRoles = roleName;
                        return View(upmodel);
                    }               
                }
            }
            return View();
        }

        // View UPDATE View of selected user.
        public ActionResult Edit(string uid)
        {
            if (uid == null)
                return HttpNotFound();

            ApplicationUser au = context.Users.First(u => u.Id == uid);
            UpdateViewModel upmodel = new UpdateViewModel();
            if(au.Roles.Count == 0)
            {
                upmodel.UserId = au.Id;
                upmodel.Firstname = au.FirstName;
                upmodel.Lastname = au.LastName;
                upmodel.PhoneNumber = au.PhoneNumber;
                upmodel.Email = au.Email;
                upmodel.UserName = au.UserName;
                return View(upmodel);
            }
            else
            {
                foreach(IdentityUserRole role in au.Roles)
                {
                    if (role != null)
                    {
                        string rid = role.RoleId;
                        string roleName = context.Roles.First(r => r.Id == role.RoleId).Name;
                        upmodel.UserId = au.Id;
                        upmodel.Firstname = au.FirstName;
                        upmodel.Lastname = au.LastName;
                        upmodel.PhoneNumber = au.PhoneNumber;
                        upmodel.Email = au.Email;
                        upmodel.UserName = au.UserName;
                        upmodel.UserRoles = roleName;
                        return View(upmodel);
                    }
                }
            }
            return View();
        }

        // UPDATE USER VIEW
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind]UpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = context.Users.First(u => u.Id == model.UserId);
                user.FirstName = model.Firstname;
                user.LastName = model.Lastname;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.UserName = model.UserName;
                //context.Entry(upmodel).State = EntityState.Modified;
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                manager.Update(user);
                context.SaveChanges();
                TempData["Success"] = "User Updated Successfully";
            }
            return RedirectToAction("UsersWithRoles", "Users");
        }

        // DELETE USER
        public ActionResult Delete(string id)
        {
            if (id == null)
                return HttpNotFound();
            ApplicationUser au = context.Users.First(u => u.Id == id);
            UpdateViewModel model = new UpdateViewModel();
            if (au == null)
                return HttpNotFound();
            model.UserId = au.Id;
            model.Firstname = au.FirstName;
            model.Lastname = au.LastName;
            model.PhoneNumber = au.PhoneNumber;
            model.UserName = au.UserName;
            model.Email = au.Email;
            if (au.Roles.Count==0)
            {
                return View(model);
            }
            else
            {
                foreach(IdentityUserRole role in au.Roles)
                {
                    if(role != null)
                    {
                        var rid = role.RoleId;
                        var roleName = context.Roles.First(r => r.Id == role.RoleId).Name;
                        model.UserRoles = roleName;
                        return View(model);
                    }
                }
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = await manager.FindByIdAsync(id);
                var rolesForUser = await manager.GetRolesAsync(id);
                if(rolesForUser.Count() > 0)
                {
                    foreach(var role in rolesForUser.ToList())
                    {
                        var result = await manager.RemoveFromRoleAsync(id, role);
                    }
                }
                await manager.DeleteAsync(user);
                TempData["Success"] = "User Deleted Successfully";
            }
            return RedirectToAction("UsersWithRoles", "Users");
        }

        // Get all Roles in SelectList
        //private List GetAllRolesAsSelectList()
        //{
        //    List<SelectListItem> lstRoles = new List<SelectListItem>();
        //    var roleManager = new RoleManager(new RoleStore(new ApplicationDbContext()));
        //    var colRoleSelectList = roleManager.Roles.OrderBy(x=>x.)
        //}
    }
}