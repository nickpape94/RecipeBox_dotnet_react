using System;

namespace RecipeBox.API.src.Main.Models
{
    public class Comment
    {

        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        // public int UserId { get; set; }

        
    }
}