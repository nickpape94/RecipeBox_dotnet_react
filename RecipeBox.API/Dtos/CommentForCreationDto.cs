using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class CommentForCreationDto
    {
        [Required]
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