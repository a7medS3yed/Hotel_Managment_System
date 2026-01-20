using HMS.Core.Contracts;
using HMS.Core.Entities.SecurityModul;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataInitializer(UserManager<HotelUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializeDataAsync()
        {
            // Seed Roles
            if (!_roleManager.Roles.Any())
            {
                var roles = new[] { "Admin", "Guest", "Sttaf" };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Seed Admin User
            if (!_userManager.Users.Any())
            {
                var defaultAdmin = new HotelUser
                {
                    FullName = "Admin Hotel",
                    UserName = "Admin_HMS",
                    Email = "Admin.HMS@gmail.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                };

                await _userManager.CreateAsync(defaultAdmin, "P@ssw0rd");
                await _userManager.AddToRoleAsync(defaultAdmin, "Admin");
            }
        }
    }
}
