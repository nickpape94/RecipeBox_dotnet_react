using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.Dtos.PostDtos;
using RecipeBox.API.Helpers;
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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<User>> GetUsers(PageParams pageParams)
        {
            var users = _context.Users.Include(p => p.UserPhotos).Include(p => p.Posts);

            return await PagedList<User>.CreateAsync(users, pageParams.PageNumber, pageParams.PageSize);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.UserPhotos).Include(p => p.Posts).Include(p => p.Favourites).FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<Comment> GetComment(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);

            return comment; 
        }

        public async Task<Post> GetPost(int id)
        {
            var post = await _context.Posts.Include(c => c.Comments).Include(p => p.PostPhoto).Include(r => r.Ratings).FirstOrDefaultAsync(x => x.PostId == id);

            return post;
        }

        // public async Task<PagedList<Post>> GetPosts(int userId, PageParams pageParams, PostForSearchDto postForSearchDto)
        // {
        //     var posts = _context.Posts.Where(x => x.UserId == userId).Include(c => c.Comments).Include(r => r.Ratings).Include(p => p.PostPhoto).OrderByDescending(x => x.Created);

        //     if (postForSearchDto.OrderBy == "most discussed")
        //         posts = posts.OrderByDescending(x => x.Comments.Count);
            
        //     if (postForSearchDto.OrderBy == "oldest")
        //         posts = posts.OrderBy(x => x.Created); 
            
        //     if (postForSearchDto.OrderBy == "newest")
        //         posts = posts.OrderByDescending(x => x.Created);
            
        //     if (postForSearchDto.OrderBy == "highest rated")
        //         posts = posts.OrderByDescending(x => x.AverageRating);
            

        //     return await PagedList<Post>.CreateAsync(posts, pageParams.PageNumber, pageParams.PageSize);
        // }

        public async Task<PagedList<Post>> GetPosts(PageParams pageParams, PostForSearchDto postForSearchDto)
        {
            var searchQuery = postForSearchDto.SearchParams.Trim().ToLower();

            var posts = _context.Posts.Include(c => c.Comments).Include(r => r.Ratings).Include(p => p.PostPhoto).OrderByDescending(x => x.Created);

            var filteredPosts = posts.Where(x => 
                x.NameOfDish.ToLower().Contains(searchQuery) || 
                x.Cuisine.ToLower().Contains(searchQuery) ||
                x.Author.ToLower().Contains(searchQuery) ||
                x.Ingredients.ToLower().Contains(searchQuery));

            if (postForSearchDto.UserId.Count() > 0)
                filteredPosts = filteredPosts.Where(x => x.UserId == int.Parse(postForSearchDto.UserId));
            
            if (postForSearchDto.OrderBy == "most discussed")
                filteredPosts = filteredPosts.OrderByDescending(x => x.Comments.Count);
            
            if (postForSearchDto.OrderBy == "oldest")
                filteredPosts = filteredPosts.OrderBy(x => x.Created); 
            
            if (postForSearchDto.OrderBy == "newest")
                filteredPosts = filteredPosts.OrderByDescending(x => x.Created);
            
            if (postForSearchDto.OrderBy == "highest rated")
                filteredPosts = filteredPosts.OrderByDescending(x => x.AverageRating);
            

            return await PagedList<Post>.CreateAsync(filteredPosts, pageParams.PageNumber, pageParams.PageSize);
        }

        // public async Task<PagedList<Post>> GetPostsByCuisine(PageParams pageParams, string cuisine)
        // {
        //     var posts = _context.Posts.Include(c => c.Comments).Include(r => r.Ratings).Include(p => p.PostPhoto).OrderByDescending(x => x.Created);

        //     var filteredPosts = posts.Where(x => x.Cuisine.ToLower() == cuisine.ToLower());

        //     return await PagedList<Post>.CreateAsync(filteredPosts, pageParams.PageNumber, pageParams.PageSize);
        // }

        public async Task<PagedList<Post>> SearchPostsByUser(PageParams pageParams, int userId)
        {
            var posts = _context.Posts.Where(x => x.UserId == userId).Include(c => c.Comments).Include(r => r.Ratings).Include(p => p.PostPhoto).OrderByDescending(x => x.Created);

            return await PagedList<Post>.CreateAsync(posts, pageParams.PageNumber, pageParams.PageSize);
        }


        public async Task<UserPhoto> GetUserPhoto(int photoId)
        {
            var photo = await _context.UserPhotos.FirstOrDefaultAsync(x => x.UserPhotoId == photoId);

            return photo;
        }

        public async Task<UserPhoto> GetMainPhotoForUser(int userId)
        {
            var photo = await _context.UserPhotos.Where(u => u.UserId == userId).FirstOrDefaultAsync();

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

        public async Task<PagedList<Post>> GetFavourites(int userId, PageParams pageParams, PostForSearchDto postForSearchDto)
        {
            var posts = _context.Posts.Include(c => c.Comments).Include(r => r.Ratings).Include(p => p.PostPhoto).OrderByDescending(x => x.Created);
            
            var favourites = _context.Favourites.Where(u => u.FavouriterId == userId);

            var filteredPosts = posts.Where(x => favourites.Any(y => y.PostId == x.PostId));
            
            if (postForSearchDto.OrderBy == "most discussed")
                filteredPosts = filteredPosts.OrderByDescending(x => x.Comments.Count);
            
            if (postForSearchDto.OrderBy == "oldest")
                filteredPosts = filteredPosts.OrderBy(x => x.Created); 
            
            if (postForSearchDto.OrderBy == "newest")
                filteredPosts = filteredPosts.OrderByDescending(x => x.Created);
            
            if (postForSearchDto.OrderBy == "highest rated")
                filteredPosts = filteredPosts.OrderByDescending(x => x.AverageRating);
            

            return await PagedList<Post>.CreateAsync(filteredPosts, pageParams.PageNumber, pageParams.PageSize);
        }

        public async Task<Favourite> GetFavourite(int postId, int userId)
        {
            var favourite = await _context.Favourites.FirstOrDefaultAsync(x => (x.FavouriterId == userId) && (x.PostId == postId));

            return favourite;
        }

        public async Task<IEnumerable<Rating>> GetRatings(int postId)
        {
            var ratings = await _context.Ratings.Where(x => x.PostId == postId).ToListAsync();

            return ratings;
        }

        public async Task<Rating> GetRating(int raterId, int postId)
        {
            var rating = await _context.Ratings.FirstOrDefaultAsync(x => x.RaterId == raterId && x.PostId == postId);

            return rating;
        }

        
    }
}