using RecipeBox.API.Models;

namespace RecipeBox.API.Dtos
{
    public class RatingsForReturnedDto
    {
        public int RatingId { get; set; }
        public double Score { get; set; }
        public User Rater { get; set; }
        public int RaterId { get; set; }
        
    }
}