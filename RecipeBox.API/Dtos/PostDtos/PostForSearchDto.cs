namespace RecipeBox.API.Dtos.PostDtos
{
    public class PostForSearchDto
    {
        public string SearchParams { get; set; }
        public string OrderBy { get; set; }
        public string UserId { get; set; }
    }
}