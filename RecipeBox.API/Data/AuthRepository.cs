using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RecipeBox.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task<User> Login(string email, string password)
        {
            
            var user = await _context.Users.Include(p => p.UserPhotos).FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return null;

            // if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //     return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using ( var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) 
            {

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> ResetPassword(int userId, string oldPassword, string newPassword)
        {
            // Check old password matches first
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            
            if (user == null)
                return null;

            // if (!VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            //     return null;


            // Old password matches, now need to write new password
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync(); 

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(x => x.Email == email))
                return true;

            return false;
        }

        
    }
}