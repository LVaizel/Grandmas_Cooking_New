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


        // JSON endpoint for AJAX polling - return lightweight DTOs to avoid circular references
        [HttpGet]
        public async Task<IActionResult> RecipesJson()
        {
            var recipes = await _recipeAPIService.GetRecipesAsync();
            var dto = (recipes ?? new List<Recipe>())
                        .Select(r => new { id = r.Id, name = r.Name })
                        .ToList();
            return Json(dto);
        }


        [HttpGet]
        public IActionResult Create1()
        {
            // Ensure view has a TempRecipe model so asp-for binds correctly
            return View(new TempRecipe());
        }

        [HttpPost]
        public IActionResult Create1(TempRecipe temp)
        {
            // Use the posted NumSteps/NumIngredients from TempRecipe
            var steps = temp?.NumSteps ?? 0;
            var ingredients = temp?.NumIngredients ?? 0;
            return RedirectToAction("Create2", new { steps = steps, ingredients = ingredients });
        }

        [HttpGet]
        public IActionResult Create2(int steps, int ingredients)
        {
            // Prepare a Recipe and a TempRecipe with counts for the view
            Recipe newRecipe = new Recipe();
            TempRecipe temp = new TempRecipe { NumIngredients = ingredients, NumSteps = steps };

            var recipeTuple = Tuple.Create(newRecipe, temp);

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
                return View(Tuple.Create(recipe, new TempRecipe()));
            }
        }

        //RecipePage shows all the details for a recipe. Ingredients and steps can be updated, as they will be inputs, their default values being what is in the model. 

        [HttpGet]
        public async Task<IActionResult> RecipePage(int id)
        {
            var recipe = await _recipeAPIService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> RecipePage(Recipe recipe)
        {
            // For simplicity, assume the entire recipe is updated
            var result = await _recipeAPIService.UpdateRecipeAsync(recipe);
            if (result)
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the recipe.");
                return View(recipe);
            }
        }
    }
}