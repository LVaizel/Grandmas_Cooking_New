// John - Temp recipe to pass to create2

using System.ComponentModel.DataAnnotations;

namespace Grandmas_Cooking_MVC.Models.RecipeModels
{
    public class TempRecipe
    {
        [Required]
        public int NumIngredients { get; set; } = 0;
        [Required]
        public int NumSteps { get; set; } = 0;
    }
}
