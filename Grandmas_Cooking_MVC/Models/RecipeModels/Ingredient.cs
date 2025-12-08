// John - Ingredient model representing a single ingredient in a recipe

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Grandmas_Cooking_MVC.Models.RecipeModels
{
    public class Ingredient
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public decimal Portion { get; set; }

        // Foreign Key
        public int RecipeId { get; set; }

        [JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}
