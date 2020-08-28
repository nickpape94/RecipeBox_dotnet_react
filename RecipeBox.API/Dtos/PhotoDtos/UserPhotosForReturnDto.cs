using System;

namespace RecipeBox.API.Dtos.PhotoDtos
{
    public class UserPhotosForReturnDto
    {
        public int UserPhotoId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
    }
}