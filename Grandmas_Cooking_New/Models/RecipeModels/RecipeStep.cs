

using System.Text.Json.Serialization;
namespace Grandmas_Cooking_API.Models.RecipeModels
{
    public class RecipeStep
    {
        public int Id { get; set; }

        public int StepNumber { get; set; } // e.g., 1, 2, 3
        public string Instruction { get; set; } = ""; // The actual text

        // Foreign Key
        public int RecipeId { get; set; }

        // Use JsonIgnore to prevent loops
        [JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}