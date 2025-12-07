using Grandmas_Cooking_MVC.Models.RecipeModels;
using static System.Net.WebRequestMethods;

namespace Grandmas_Cooking_MVC.InfrastructureLayer
{
    public class RecipeAPIService
    {

        private HttpClient httpClient;

        public RecipeAPIService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // get data

        public async Task<List<Recipe>?> GetRecipesAsync()
        {
            var url = "https://localhost:7137/api/RecipeAPI/recipes";

            var result = await httpClient.GetAsync(url);

            if (result.Content.Headers.ContentLength == 0) return new List<Recipe>();

            var books = await result.Content.ReadFromJsonAsync<List<Recipe>>();

            return books;
        }

        // Create recipe
        public async Task<bool> CreateRecipeAsync(Recipe recipe)
        {
            var url = "https://localhost:7137/api/RecipeAPI/recipes";
            var result = await httpClient.PostAsJsonAsync(url, recipe);
            return result.IsSuccessStatusCode;
        }

        // get user's recipes

        public async Task<List<Recipe>?> GetUserRecipesAsync()
        {
            var url = "https://localhost:7137/api/RecipeAPI/my-recipes";
            var result = await httpClient.GetAsync(url);
            if (result.Content.Headers.ContentLength == 0) return new List<Recipe>();
            var books = await result.Content.ReadFromJsonAsync<List<Recipe>>();
            return books;
        }

        // get the API reciepe by id

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{id}";
            var result = await httpClient.GetAsync(url);
            if (result.Content.Headers.ContentLength == 0) return null;
            var recipe = await result.Content.ReadFromJsonAsync<Recipe>();
            return recipe;
        }

        // Delete recipe by id

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/{id}";
            var result = await httpClient.DeleteAsync(url);
            return result.IsSuccessStatusCode;
        }

        // Add new ingredient to recipe

        public async Task<bool> AddIngredientAsync(int recipeId, Ingredient ingredient)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{recipeId}/ingredients";
            var result = await httpClient.PostAsJsonAsync(url, ingredient);
            return result.IsSuccessStatusCode;
        }

        // Add new step to recipe

        public async Task<bool> AddStepAsync(int recipeId, RecipeStep step)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{recipeId}/steps";
            var result = await httpClient.PostAsJsonAsync(url, step);
            return result.IsSuccessStatusCode;
        }



    }   
}
