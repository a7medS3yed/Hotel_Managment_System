using HMS.Core.Entities.BookingModule;
using HMS.Core.Entities.SecurityModul;
using HMS.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.Context
{
    public class HMSDbContext : IdentityDbContext<HotelUser>
    {
        public HMSDbContext(DbContextOptions<HMSDbContext> options) 
            : base(options) {}

        override protected void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HotelUser>().ToTable("Users");
            modelBuilder.Entity<StaffUser>().ToTable("StaffUsers");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<Booking>().Property(B => B.TotalAmount).HasPrecision(8, 2);

            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
