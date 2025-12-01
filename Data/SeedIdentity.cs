using System;
using System.Threading.Tasks;
using BeanScene.Web.Models;                  // 👈 for ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BeanScene.Web.Data
{
    public static class SeedIdentity
    {
        public static async Task EnsureSeededAsync(IServiceProvider services)
        {
            // We are already passing a scope's ServiceProvider from Program.cs,
            // so just resolve managers from it:
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1️⃣ Ensure roles exist
            string[] roles = new[] { "Admin", "Staff", "Member" };

            foreach (var r in roles)
            {
                if (!await roleMgr.RoleExistsAsync(r))
                {
                    await roleMgr.CreateAsync(new IdentityRole(r));
                }
            }

            // 2️⃣ Ensure Admin user exists
            const string adminEmail = "admin@beanscene.local";
            const string adminPass = "Admin!234"; // change after first login

            var existingAdmin = await userMgr.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",       // optional
                    LastName = "Admin"         // optional
                };

                var createAdmin = await userMgr.CreateAsync(admin, adminPass);
                if (createAdmin.Succeeded)
                {
                    await userMgr.AddToRoleAsync(admin, "Admin");
                }
            }

            // 3️⃣ (Optional) Seed a Staff user for testing
            const string staffEmail = "staff@beanscene.local";
            const string staffPass = "Staff!234";

            var existingStaff = await userMgr.FindByEmailAsync(staffEmail);
            if (existingStaff == null)
            {
                var staff = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true,
                    FirstName = "Front",
                    LastName = "Staff"
                };

                var createStaff = await userMgr.CreateAsync(staff, staffPass);
                if (createStaff.Succeeded)
                {
                    await userMgr.AddToRoleAsync(staff, "Staff");
                }
            }
        }
    }
}