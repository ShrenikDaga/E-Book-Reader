using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Lab10.Data
{
    public class Seed
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var SignInManager = serviceProvider.GetRequiredService<SignInManager<IdentityUser>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var rolename in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(rolename);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            var poweruser = new IdentityUser
            {
                UserName = configuration.GetSection("AppSettings")["UserEmail"],
                Email = configuration.GetSection("AppSettings")["UserEmail"]
            };

            String userpassword = configuration.GetSection("AppSettings")["UserPassword"];
            var user = await UserManager.FindByEmailAsync(configuration.GetSection("AppSettings")["UserEmail"]);

            if (user == null)
            {
                var createpoweruser = await UserManager.CreateAsync(poweruser, userpassword);
                if (createpoweruser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(poweruser, "Admin");
                    
                }
            }
        }
    }
}
