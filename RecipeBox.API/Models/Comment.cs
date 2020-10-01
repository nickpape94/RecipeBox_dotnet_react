using System;

namespace RecipeBox.API.Models
{
    public class Comment
    {

        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string UserPhotoUrl { get; set; }
        public User Commenter { get; set; }
        public int CommenterId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        
        

        
    }
}