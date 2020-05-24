using System;
using System.Collections.Generic;

namespace RecipeBox.API.src.Main.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string NameOfDish { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Method { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string Feeds { get; set; }
        public string Cuisine { get; set; }
        public DateTime Created { get; set; }
        // public ICollection<PostPhoto> PostPhoto { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        
       
        
    }
}