using Microsoft.AspNetCore.Identity;

namespace Grandmas_Cooking_API.Models
{
    public class AppUser : IdentityUser
    {
        public String Name { get; set; } = "";
        public String FullName { get; set; } = "";
        List<RecipeModels.Recipe> Recipes { get; set; } = new List<RecipeModels.Recipe>();
    }
}
