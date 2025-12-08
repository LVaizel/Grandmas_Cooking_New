using System.ComponentModel.DataAnnotations;

namespace Grandmas_Cooking_MVC.Models
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
