using Grandmas_Cooking_API.Models.RecipeModels;
using Grandmas_Cooking_API.ServiceLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecipeAPIController : ControllerBase
{
    private readonly RecipeService _recipeService;

    public RecipeAPIController(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet("recipes")]
    public async Task<IActionResult> GetRecipes()
    //public async Task<IList<Recipe>> GetRecipes()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        return Ok(recipes);
        //return recipes;
    }

    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetUserRecipes()
    {
        // Get User ID from Token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var recipes = await _recipeService.GetRecipesByUserIdAsync(userId);
        return Ok(recipes);
    }

    [HttpGet("recipes/{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
        if (recipe == null) return NotFound();

        // Ingredients are already included now!
        return Ok(recipe);
    }

    [HttpPost("recipes")]
    public async Task<IActionResult> Create([FromBody] Recipe recipe)
    {
        // Automatically assign the logged-in user to the recipe
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        recipe.UserId = userId;

        await _recipeService.AddRecipeAsync(recipe);
        return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        await _recipeService.DeleteRecipeAsync(id);
        return NoContent();
    }

    [HttpPost("recipes/{recipeId}/ingredients")]
    public async Task<IActionResult> AddIngredientAsync(int recipeId, [FromBody] Ingredient ingredient)
    {

        var result = await _recipeService.AddIngredientAsync(recipeId, ingredient);

        if (result == null)
        {
            return NotFound("Recipe not found.");
        }

        return Ok(result);
    }

    [HttpPost("recipes/{recipeId}/steps")]
    public async Task<IActionResult> AddStep(int recipeId, [FromBody] RecipeStep step)
    {
        // Optional: Validation
        if (string.IsNullOrWhiteSpace(step.Instruction))
        {
            return BadRequest("Instruction text cannot be empty.");
        }

        var result = await _recipeService.AddStepAsync(recipeId, step);

        if (result == null)
        {
            return NotFound("Recipe not found.");
        }

        return Ok(result);
    }

    [HttpPut("recipes/{recipeId}/update")]
    public async Task<IActionResult> UpdateRecipe(int recipeId, [FromBody] Recipe updated)
    {
        if (recipeId != updated.Id)
        {
            return NotFound("Recipe not found.");
        }

        var result = await _recipeService.UpdateRecipeAsync(updated);

        return Ok(result);
    }
}