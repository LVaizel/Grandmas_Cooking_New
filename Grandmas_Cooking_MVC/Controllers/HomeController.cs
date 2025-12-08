// John, Gurvansh, Gaganvir - HomeController.cs created for handling home page, authentication, recipe management, and chat functionality.

using Grandmas_Cooking_MVC.Data;
using Grandmas_Cooking_MVC.InfrastructureLayer;
using Grandmas_Cooking_MVC.Models;
using Grandmas_Cooking_MVC.Models.ChatModels;
using Grandmas_Cooking_MVC.Models.RecipeModels;
using Grandmas_Cooking_MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Grandmas_Cooking_MVC.Controllers
{
    public class HomeController : Controller
    {
        private RecipeAPIService _recipeAPIService;
        private AuthApiService _authApiService;
        private readonly  OpenAiService _openAiService;
        private readonly AppDbContext _context;

        public HomeController(RecipeAPIService recipeApiService, AuthApiService authApiService, OpenAiService openAiService,AppDbContext appDbContext)
        {
            _recipeAPIService = recipeApiService;
            _authApiService = authApiService;
            _openAiService = openAiService;
            _context = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> LoginPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPage(LoginRequest _loginDto)
        {
            var authResponse = await _authApiService.GetAuthResponse(_loginDto);

            if ( _authApiService.GetTokenFromSession() != null )
            {
                return RedirectToAction("HomePage");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginPage");
        }

        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPage(RegisterRequest request)
        {
            var ok = await _authApiService.RegisterAsync(request);
            if ( ok )
            {
                return RedirectToAction("LoginPage");
            }

            ModelState.AddModelError(string.Empty, "Registration failed.");
            return View(request);
        }

        public async Task<IActionResult> HomePage()
        {
            // Always fetch updated lists for (all, mine) to avoid stale data
            var all = await _recipeAPIService.GetRecipesAsync() ?? new List<Recipe>();
            var mine = await _recipeAPIService.GetUserRecipesAsync() ?? new List<Recipe>();
            var model = Tuple.Create(all, mine);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ViewRecipe(int RecipeId)
        {
            var recipe = await _recipeAPIService.GetRecipeByIdAsync(RecipeId);
            if ( recipe == null )
            {
                return NotFound();
            }
            var model = recipe;
            return View(model);
        }

        // JSON endpoint for AJAX polling - return lightweight DTOs to avoid circular references
        [HttpGet]
        public async Task<IActionResult> RecipesJson()
        {
            var recipes = await _recipeAPIService.GetRecipesAsync();
            var dto = ( recipes ?? new List<Recipe>() )
                        .Select(r => new { id = r.Id, name = r.Name })
                        .ToList();
            return Json(dto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create1()
        {
            // Ensure view has a TempRecipe model so asp-for binds correctly
            return View(new TempRecipe());
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create1(TempRecipe temp)
        {
            // Use the posted NumSteps/NumIngredients from TempRecipe
            var steps = temp?.NumSteps ?? 0;
            var ingredients = temp?.NumIngredients ?? 0;
            return RedirectToAction("Create2", new { steps = steps, ingredients = ingredients });
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create2(int steps, int ingredients)
        {
            // Prepare a Recipe and a TempRecipe with counts for the view
            Recipe newRecipe = new Recipe();
            for ( int i = 0; i < steps; i++ )
            {
                newRecipe.RecipeSteps.Add(new RecipeStep());
            }
            for ( int i = 0; i < ingredients; i++ )
            {
                newRecipe.Ingredients.Add(new Ingredient());
            };

            return View(newRecipe);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create2(Recipe recipe)
        {
            var result = await _recipeAPIService.CreateRecipeAsync(recipe);
            if ( result )
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the recipe.");
                return View(recipe);
            }
        }

        //RecipePage shows all the details for a recipe. Ingredients and steps can be updated, as they will be inputs, their default values being what is in the model. 
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RecipePage(int id)
        {
            var recipe = await _recipeAPIService.GetRecipeByIdAsync(id);
            if ( recipe == null )
            {
                return NotFound();
            }
            return View(recipe);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RecipePage(Recipe recipe)
        {
            // Fetch existing full recipe first so we don't accidentally wipe fields if collections failed to bind
            var existing = await _recipeAPIService.GetRecipeByIdAsync(recipe.Id);
            if ( existing == null )
            {
                return NotFound();
            }

            // Update editable top-level fields
            existing.Description = recipe.Description;
            existing.ImageUrl = recipe.ImageUrl;
            // Name remains read-only in view; keep existing.Name

            // Only replace collections if they were posted; otherwise keep existing
            existing.Ingredients = recipe.Ingredients ?? existing.Ingredients;
            existing.RecipeSteps = recipe.RecipeSteps ?? existing.RecipeSteps;

            var result = await _recipeAPIService.UpdateRecipeAsync(existing);
            if ( result )
            {
                // Reload the page via GET so full recipe data (name/description/etc.) is fetched from API and displayed
                return RedirectToAction("RecipePage", new { id = recipe.Id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the recipe.");
                // Fetch full recipe from API so view has all fields (top section) populated
                var full = await _recipeAPIService.GetRecipeByIdAsync(recipe.Id);
                if ( full == null )
                {
                    return NotFound();
                }
                return View(full);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var ok = await _recipeAPIService.DeleteRecipeAsync(id);
            return RedirectToAction("HomePage");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Chat()
        {
            var model = new ChatViewModel
            {
                ChatHistory = _context.ChatMessages.OrderBy(cm => cm.TimeStamp).ToList()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Chat(ChatViewModel chatViewModel)
        {
            var userMessage = new ChatMessage
            {
                Sender = "User",
                Text = chatViewModel.UserMessage,
            };
            _context.ChatMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            var response = await _openAiService.GetChatResponseAsync(chatViewModel.UserMessage);

            var botMessage = new ChatMessage
            {
                Sender = "Bot",
                Text = response,
            };
            _context.ChatMessages.Add(botMessage);
            await _context.SaveChangesAsync();

            chatViewModel.ChatHistory = _context.ChatMessages.OrderBy(cm => cm.TimeStamp).ToList();
            chatViewModel.UserMessage = string.Empty;
            return View(chatViewModel);
        }
    }
}