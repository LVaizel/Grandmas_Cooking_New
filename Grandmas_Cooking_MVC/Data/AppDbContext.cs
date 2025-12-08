using Grandmas_Cooking_MVC.Models.ChatModels;
using Microsoft.EntityFrameworkCore;

namespace Grandmas_Cooking_MVC.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
