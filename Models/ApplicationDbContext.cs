using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace E_CommerceApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<Category> Catrgories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }    
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

            builder.Entity<Product>(entity =>
            {
                entity.Property(product => product.Description).IsRequired().HasMaxLength(2000);
                entity.Property(product => product.Price).IsRequired().HasColumnType("decimal(9,2)").HasDefaultValue(0.00);
                entity.Property(product => product.Stock).IsRequired().HasDefaultValue(0);
                entity.Property(product => product.Name).IsRequired().HasMaxLength(100);

            });

            builder.Entity<Tag>().Property(tag => tag.Name).IsRequired().HasMaxLength(50);
            builder.Entity<Tag>().HasIndex(tag => tag.Name).IsUnique();

            builder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId }); // composite key

            builder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId); //One-To-Many from product to producttags
                                                    //Product has many rows in producttags but each producttag belongs to one product

            builder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId); //One-To-Many from tag to producttags
                                                //Tag has many rows in producttags but each producttag belongs to one tag

            builder.Entity<Review>(entity =>
            {
                entity.Property(review => review.Description).IsRequired().HasMaxLength(2000);
                entity.Property(review => review.IsEdited).IsRequired().HasDefaultValue(false);
                entity.Property(review => review.RateValue).IsRequired();
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Review_RateValue",
                    "[RateValue] BETWEEN 1 AND 5"
                 ));
                
            });

            builder.Entity<Review>()
                .HasOne(review => review.User)
                .WithMany(user => user.Reviews)
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(review => review.Product)
                .WithMany(product => product.Reviews)
                .HasForeignKey(review => review.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>().Property(category => category.Name).IsRequired().HasMaxLength(50);
            builder.Entity<Category>().HasIndex(category => category.Name).IsUnique();

            builder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId }); // composite key

            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId); //One-To-Many from product to productcategory
                                                    //Product has many rows in productcategories but each productcategory belongs to one product

            builder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId); //One-To-Many from category to productcategory
                                                     //Category has many rows in productcategories but each productcategory belongs to one category

        }
    }
}
