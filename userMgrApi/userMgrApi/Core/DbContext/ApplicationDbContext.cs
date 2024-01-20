using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using userMgrApi.Core.Entities;

namespace userMgrApi.Core.DbContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
		{

		}

		public DbSet<Log> Logs { get; set; }

        public DbSet<Message> Messages { get; set; }

        //overide the name of identity model
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //config
            builder.Entity<ApplicationUser>(e =>
            {
                e.ToTable("Users");
            });

            builder.Entity<IdentityUserClaim<string>>(e =>
            {
                e.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(e =>
            {
                e.ToTable("UserLogin");
            });

            builder.Entity<IdentityUserToken<string>>(e =>
            {
                e.ToTable("UserTokens");
            });

            builder.Entity<IdentityRole>(e =>
            {
                e.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<string>>(e =>
            {
                e.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(e =>
            {
                e.ToTable("UserRoles");
            });





        }

    }
}

