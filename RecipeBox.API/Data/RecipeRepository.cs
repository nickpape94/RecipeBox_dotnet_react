using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly DataContext _context;
        public RecipeRepository(DataContext context)
        {
            _context = context;
            
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public async Task<Post> GetPost(int id)
        {
            var post = await _context.Posts.Include(c => c.Comments).Include(p => p.PostPhoto).FirstOrDefaultAsync(x => x.PostId == id);

            return post;
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            var posts = await _context.Posts.Include(c => c.Comments).ToListAsync();

            return posts;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.UserPhotos).Include(p => p.Posts).Include(p => p.Favourites).FirstOrDefaultAsync(x => x.UserId == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(p => p.UserPhotos).Include(p => p.Posts).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Comment> GetComment(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);

            return comment; 
        }

        public async Task<UserPhoto> GetUserPhoto(int photoId)
        {
            var photo = await _context.UserPhotos.FirstOrDefaultAsync(x => x.UserPhotoId == photoId);

            return photo;
        }

        public async Task<UserPhoto> GetMainPhotoForUser(int userId)
        {
            var photo = await _context.UserPhotos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);

            return photo;
        }

        public async Task<PostPhoto> GetPostPhoto(int photoId)
        {
            var photo = await _context.PostPhotos.FirstOrDefaultAsync(p => p.PostPhotoId == photoId);

            return photo;
        }

        public async Task<PostPhoto> GetMainPhotoForPost(int postId)
        {
            var photo = await _context.PostPhotos.Where(u => u.PostId == postId).FirstOrDefaultAsync(p => p.IsMain);

            return photo;
        }

        public async Task<IEnumerable<Favourite>> GetFavourites(int userId)
        {
            
            var favourites = await _context.Favourites.Where(u => u.FavouriterId == userId).ToListAsync();

            return favourites;
        }

        
    }
}