
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) 
        {  
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("UsersAccount");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");


            builder.Entity<AppUser>(b =>
            {
                b.Property(u => u.NormalizedUserName).HasMaxLength(191);
                b.Property(u => u.NormalizedEmail).HasMaxLength(191);
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.Property(r => r.NormalizedName).HasMaxLength(191);
            });
        }
        public DbSet<FeedBack> FeedBack { get; set; }

        public DbSet<Coupons> Coupons { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Store> Stores { get; set; }

       

        public DbSet<Offers> Offers { get; set; }
         
        public DbSet<CouponsOffers> CouponsOffers { get; set; }

    }
}
