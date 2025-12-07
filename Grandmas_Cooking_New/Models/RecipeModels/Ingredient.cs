using System.Text.Json.Serialization;

namespace Grandmas_Cooking_API.Models.RecipeModels
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Portion { get; set; }

        // Foreign Key
        public int RecipeId { get; set; }

        [JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}