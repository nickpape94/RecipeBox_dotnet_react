using System;
using System.ComponentModel.DataAnnotations;
using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos.CommentDtos
{
    public class CommentForUpdateDto
    {
        [StringLength(300, MinimumLength = 6, ErrorMessage = "Comment must be between 6 and 300 characters long")]
        public string Text { get; set; }
    }
}