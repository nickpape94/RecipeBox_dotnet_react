using System;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Dtos
{
    public class CommentForCreationDto
    {
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        public int CommenterId { get; set; }
        // public Post Commenter { get; set; }
        // public int PostId { get; set; }

        public CommentForCreationDto()
        {
            Created = DateTime.Now;
        }
    }
}