using Microsoft.EntityFrameworkCore;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Data
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
        //     builder.Entity<Post>()
        //         .
        // }
        
    }
}