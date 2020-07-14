namespace RecipeBox.API.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int Score { get; set; }
        public User Rater { get; set; }
        public int RaterId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
    }
}