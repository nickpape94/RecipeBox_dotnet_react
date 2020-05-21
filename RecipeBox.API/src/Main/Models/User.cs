using System;
using System.Collections.Generic;

namespace RecipeBox.API.src.Main.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<UserPhoto> UserPhotos { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}