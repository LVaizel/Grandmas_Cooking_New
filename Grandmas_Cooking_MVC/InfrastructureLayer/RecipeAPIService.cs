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
            var url = "https://localhost:7137/API/RecipeAPI/";

            var result = await httpClient.GetAsync(url);

            if (result.Content.Headers.ContentLength == 0) return new List<Book>();

            var books = await result.Content.ReadFromJsonAsync<List<Book>>();

            return books;
        }
}
