using System;
using Microsoft.AspNetCore.Http;

namespace RecipeBox.API.Dtos
{
    public class UserPhotoForCreationDto
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }

        public UserPhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}