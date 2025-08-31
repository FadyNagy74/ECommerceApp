using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(user => user.UserName).IsRequired().HasMaxLength(50);
                //Normalized UserName contains an Index which by default makes UserName Unique so when you search for
                //user using userManager.FindByNameAsync() Identity searches through the NormalizedUserName by default

                entity.Property(user => user.Email).IsRequired().HasMaxLength(100);
                //If you want to enforce a regex or a format you have to enforce it in a DTO (App level)

                entity.HasIndex(user => user.NormalizedEmail).IsUnique();
                //To make our Email unique we must add an index to the NormalizedEmail as Identity looks up emails using
                //the normalized Email
                entity.Property(user => user.FirstName).IsRequired().HasMaxLength(100);

                entity.Property(user => user.LastName).IsRequired().HasMaxLength(100);

                entity.HasIndex(user =>user.PhoneNumber).IsUnique();
                //If you want to enforce a regex or a format you have to enforce it in a DTO (App level)

                entity.Property(user => user.PhoneNumber).IsRequired();

                entity.Property(user => user.JoinDate).IsRequired();


            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(role => role.Name).IsRequired().HasMaxLength(20);
                //Normalized name is unqiue so no need to add an Index
            }
            );

            builder.Entity<ApplicationUser>()
                .HasOne(user => user.City)
                .WithMany(city => city.Users)
                .HasForeignKey(user => user.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserAddress>()
                .HasOne(userAddress => userAddress.User)
                .WithMany(user => user.UserAddresses)
                .HasForeignKey(userAddress => userAddress.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserAddress>(entity =>
            {
                entity.Property(userAddress => userAddress.Address).IsRequired();
                entity.HasIndex(userAddress => userAddress.Address).IsUnique();
            }
            );
        }
    }
}
