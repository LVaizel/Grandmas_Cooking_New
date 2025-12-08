// Gurvansh - RegisterRequest.cs created for representing user registration request data.

using System.ComponentModel.DataAnnotations;

namespace Grandmas_Cooking_MVC.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
