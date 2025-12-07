using Grandmas_Cooking_MVC.InfrastructureLayer;
using Grandmas_Cooking_MVC.Models;
using Grandmas_Cooking_MVC.Models.RecipeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Grandmas_Cooking_MVC.Controllers
{
    public class HomeController : Controller
    {
        private RecipeAPIService _recipeAPIService;
        private AuthApiService _authApiService;

        public HomeController(RecipeAPIService recipeApiService, AuthApiService authApiService)
        {
            _recipeAPIService = recipeApiService;
            _authApiService = authApiService;
        }

        [HttpGet]
        public async Task<IActionResult> LoginPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPage(LoginRequest _loginDto)
        {
            // Minimal placeholder: redirect to HomePage after POST
            var authResponse = await _authApiService.GetAuthResponse(_loginDto);

            return RedirectToAction("HomePage");
        }

        public async Task<IActionResult> HomePage()
        {
            var recipes = await _recipeAPIService.GetRecipesAsync();
            return View(recipes);
        }


        //// JSON endpoint for AJAX polling - return lightweight DTOs to avoid circular references
        //[HttpGet]
        //public async Task<IActionResult> RecipesJson()
        //{
        //    var recipes = await _recipeAPIService.GetRecipesAsync();
        //    var dto = (recipes ?? new List<Recipe>())
        //                .Select(r => new { id = r.Id, name = r.Name })
        //                .ToList();
        //    return Json(dto);
        //}


        [HttpGet]
        public async Task<IActionResult> Create1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create1(int steps, int ingredients)
        {
            
            return RedirectToAction("Create2", new { steps, ingredients });
        }

        [HttpGet]
        public async Task<IActionResult> Create2(int steps, int ingredients)
        {
            Recipe newRecipe = new Recipe();

            var recipeTuple = ( newRecipe, steps, ingredients );

            return View(recipeTuple);
        }

        [HttpPost]
        public async Task<IActionResult> Create2(Recipe recipe)
        {
            var result = await _recipeAPIService.CreateRecipeAsync(recipe);
            if (result)
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the recipe.");
                return View(recipe);
            }
        }
    }
}
