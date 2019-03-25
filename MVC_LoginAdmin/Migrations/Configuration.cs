namespace MVC_LoginAdmin.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MVC_LoginAdmin.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVC_LoginAdmin.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MVC_LoginAdmin.Models.ApplicationDbContext";
        }

        protected override void Seed(MVC_LoginAdmin.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            // CREATE Role 'Admin'
            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }
            // Create Role 'Guest'
            if (!context.Roles.Any(r => r.Name == "Guest"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Guest" };

                manager.Create(role);
            }


            // TEST USER 'tester' -- No Role yet.
            if (!context.Users.Any(u => u.UserName == "tester"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "tester" };

                manager.Create(user, "password");
                manager.AddToRole(user.Id, "Guest");
            }
            // ADMIN USER 'admin' -- Role yes.
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@email.com"                    
                };
                manager.Create(user, "password");
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
