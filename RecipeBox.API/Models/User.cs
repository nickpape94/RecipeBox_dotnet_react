using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RecipeBox.API.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string About { get; set; }
        public virtual ICollection<UserPhoto> UserPhotos { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}