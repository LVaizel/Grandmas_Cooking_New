using Grandmas_Cooking_API.Models;
using Grandmas_Cooking_API.Models.RecipeModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Grandmas_Cooking_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Recipe> Recipe { get; set; }
    }
}
