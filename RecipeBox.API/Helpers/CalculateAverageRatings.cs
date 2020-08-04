using System;
using System.Linq;
using System.Threading.Tasks;
using RecipeBox.API.Data;

namespace RecipeBox.API.Helpers
{
    public class CalculateAverageRatings
    {
        private readonly IRecipeRepository _repo;
        public CalculateAverageRatings(IRecipeRepository repo)
        {
            _repo = repo;
            
        }
        public async Task<double> GetAverageRating(int postId)
        {
            var ratingsForPost = await _repo.GetRatings(postId);

            double sum = 0;
            double numberOfRatings = ratingsForPost.Count();
            
            foreach (var rating in ratingsForPost)
            {
                sum += rating.Score;
            }

            if (numberOfRatings == 0) return 0.0;
            
            var averageRating = sum / numberOfRatings;

            return Math.Round(averageRating, 2);
        }
    }
}