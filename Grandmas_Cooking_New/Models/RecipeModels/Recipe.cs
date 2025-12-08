

namespace Grandmas_Cooking_API.Models.RecipeModels
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; } = "";
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<RecipeStep> RecipeSteps { get; set; } = new List<RecipeStep>();
        public string? ImageUrl { get; set; } = null;
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
