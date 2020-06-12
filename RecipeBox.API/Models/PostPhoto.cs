using System;

namespace RecipeBox.API.Models
{
    public class PostPhoto
    {
        public int PostPhotoId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
    }
}