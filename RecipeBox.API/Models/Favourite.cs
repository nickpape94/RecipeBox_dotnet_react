using System;
using System.Collections.Generic;

namespace RecipeBox.API.Models
{
    public class Favourite
    {
        public int Id { get; set; }
        public int FavouriterId { get; set; }
        public User Favouriter { get; set; }
        public string Username { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }

    }
}