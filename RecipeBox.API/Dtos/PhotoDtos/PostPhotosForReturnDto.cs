using System;

namespace RecipeBox.API.Dtos.PhotoDtos
{
    public class PostPhotosForReturnDto
    {
        public int PostPhotoId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
    }
}