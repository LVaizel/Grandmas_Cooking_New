using Grandmas_Cooking_MVC.Models.RecipeModels;
using static System.Net.WebRequestMethods;

namespace Grandmas_Cooking_MVC.InfrastructureLayer
{
    public class RecipeAPIService
    {

        private HttpClient httpClient;
        private readonly AuthApiService authApiService;

        public RecipeAPIService(HttpClient httpClient, AuthApiService authApiService)
        {
            this.httpClient = httpClient;
            this.authApiService = authApiService;
        }

        // get data

        public async Task<List<Recipe>?> GetRecipesAsync()
        {
            var url = "https://localhost:7137/api/RecipeAPI/recipes";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.GetAsync(url);

            if (result.Content.Headers.ContentLength == 0) return new List<Recipe>();

            var books = await result.Content.ReadFromJsonAsync<List<Recipe>>();

            return books;
        }

        // Create recipe
        public async Task<bool> CreateRecipeAsync(Recipe recipe)
        {
                var url = "https://localhost:7137/api/RecipeAPI/recipes";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.PostAsJsonAsync(url, recipe);
            return result.IsSuccessStatusCode;
        }

        // Update recipe - must call API's /recipes/{id}/update
        public async Task<bool> UpdateRecipeAsync(Recipe recipe)
        {
            if (recipe == null) return false;

            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{recipe.Id}/update";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.PutAsJsonAsync(url, recipe);
            return result.IsSuccessStatusCode;
        }

        // get user's recipes

        public async Task<List<Recipe>?> GetUserRecipesAsync()
        {
            var url = "https://localhost:7137/api/RecipeAPI/my-recipes";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.GetAsync(url);
            if (result.Content.Headers.ContentLength == 0) return new List<Recipe>();
            var books = await result.Content.ReadFromJsonAsync<List<Recipe>>();
            return books;
        }

        // get the API reciepe by id

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{id}";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.GetAsync(url);
            if (result.Content.Headers.ContentLength == 0) return null;
            var recipe = await result.Content.ReadFromJsonAsync<Recipe>();
            return recipe;
        }

        // Delete recipe by id

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            // API route is DELETE /api/RecipeAPI/{id}
            var url = $"https://localhost:7137/api/RecipeAPI/{id}";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.DeleteAsync(url);
            return result.IsSuccessStatusCode;
        }

        // Add new ingredient to recipe

        public async Task<bool> AddIngredientAsync(int recipeId, Ingredient ingredient)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{recipeId}/ingredients";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.PostAsJsonAsync(url, ingredient);
            return result.IsSuccessStatusCode;
        }

        // Add new step to recipe

        public async Task<bool> AddStepAsync(int recipeId, RecipeStep step)
        {
            var url = $"https://localhost:7137/api/RecipeAPI/recipes/{recipeId}/steps";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authApiService.GetTokenFromSession());

            var result = await httpClient.PostAsJsonAsync(url, step);
            return result.IsSuccessStatusCode;
        }
    }   
}
