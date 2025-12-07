using Grandmas_Cooking_API.Data;
using Grandmas_Cooking_API.Models;
using Grandmas_Cooking_API.Models.RecipeModels;
using Microsoft.EntityFrameworkCore;

namespace Grandmas_Cooking_API.ServiceLayer
{
    public class RecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipe
                .Include(r => r.Ingredients)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecipesByUserIdAsync(string userId)
        {
            return await _context.Recipe
                .Where(r => r.UserId == userId)
                .Include(r => r.Ingredients)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipe
                .Include(r => r.Ingredients)
                .Include(r => r.RecipeSteps.OrderBy(s => s.StepNumber)) // Load and Order Steps
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Update the Create method to handle incoming steps
        public async Task AddRecipeAsync(Recipe recipe)
        {
            // If the client sends steps without numbers, auto-number them
            if (recipe.RecipeSteps != null)
            {
                for (int i = 0; i < recipe.RecipeSteps.Count; i++)
                {
                    recipe.RecipeSteps[i].StepNumber = i + 1;
                }
            }

            _context.Recipe.Add(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            if ( _context.Recipe.Any(r => r.Id == recipe.Id) )
            {
                _context.Recipe.Update(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if ( recipe != null )
            {
                _context.Recipe.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Ingredient?> AddIngredientAsync(int recipeId, Ingredient ingredient)
        {
            // 1. Verify the recipe exists
            var recipeExists = await _context.Recipe.AnyAsync(r => r.Id == recipeId);
            if ( !recipeExists ) return null;

            // 2. Link the ingredient to the recipe
            ingredient.RecipeId = recipeId;

            // 3. Add to the DbContext directly
            _context.Set<Ingredient>().Add(ingredient);
            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<RecipeStep?> AddStepAsync(int recipeId, RecipeStep step)
        {
            var recipeExists = await _context.Recipe.AnyAsync(r => r.Id == recipeId);
            if ( !recipeExists ) return null;

            // Check the database for the highest StepNumber currently assigned to this recipe.
            var currentMaxStep = await _context.Set<RecipeStep>()
                .Where(s => s.RecipeId == recipeId)
                .MaxAsync(s => ( int? ) s.StepNumber) ?? 0; // Returns 0 if no steps exist

            step.StepNumber = currentMaxStep + 1;
            step.RecipeId = recipeId;

            // 3. Save
            _context.Set<RecipeStep>().Add(step);
            await _context.SaveChangesAsync();

            return await _context.Set<RecipeStep>()
                .Include(s => s.Recipe)
                .FirstOrDefaultAsync(s => s.StepNumber == step.StepNumber && s.RecipeId == recipeId);
        }
    }
}