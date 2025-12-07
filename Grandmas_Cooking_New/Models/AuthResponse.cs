namespace Grandmas_Cooking_API.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public string Name { get; set; } = "";
        public string UserName { get; set; } = "";
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
