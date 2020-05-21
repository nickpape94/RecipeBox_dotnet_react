using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Data
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

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.UserPhotos).FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(p => p.UserPhotos).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}