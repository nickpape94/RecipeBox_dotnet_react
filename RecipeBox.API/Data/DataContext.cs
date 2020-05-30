using Microsoft.EntityFrameworkCore;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options ): base(options) {}
        
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostPhoto> PostPhotos { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        public DbSet<Post> Posts { get; set; }

        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     // builder.Entity<Comment>()
        //     //     .HasKey(k => new {k.UserId, k.PostId});

        //     builder.Entity<Comment>()
        //         .HasOne(c => c.Post)
        //         .WithMany(b => b.Comments)
        //         .HasForeignKey(b => b.PostId);
            
        //     builder.Entity<Comment>()
        //         .HasOne(c => c.User)
        //         .WithOne(b => b.Posts)
        //         .HasForeignKey(b => b.UserId);
        // }
        
    }
}