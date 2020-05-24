using System;

namespace RecipeBox.API.src.Main.Models
{
    public class PostPhoto
    {
        public int PostPhotoId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}