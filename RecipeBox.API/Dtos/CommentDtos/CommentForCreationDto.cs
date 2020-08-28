using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.CommentDtos
{
    public class CommentForCreationDto
    {
        [Required]
        [StringLength(300, MinimumLength = 6, ErrorMessage = "Comment must be between 6 and 300 characters long")]
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