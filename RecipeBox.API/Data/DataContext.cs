using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options ): base(options) {}
        
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostPhoto> PostPhotos { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole => 
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
                
                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            }); 
            //     builder.Entity<Favourite>()
            //         .HasKey(k => new {k.UserId, k.PostId});

            //     builder.Entity<Comment>()
            //         .HasOne(c => c.Post)
            //         .WithMany(b => b.Comments)
            //         .HasForeignKey(b => b.PostId);
                
            //     builder.Entity<Comment>()
            //         .HasOne(c => c.User)
            //         .WithOne(b => b.Posts)
            //         .HasForeignKey(b => b.UserId);
        }
        
    }
}