using System.Runtime.CompilerServices;
using Grandmas_Cooking_MVC.Models;
using Microsoft.AspNetCore.Http;

namespace Grandmas_Cooking_MVC.InfrastructureLayer
{
    public class AuthApiService
    {
        private HttpClient httpClient;
        private readonly IHttpContextAccessor _accessor;

        public AuthApiService(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            this.httpClient = httpClient;
            _accessor = accessor;
        }

        // get the auth token from the RecipeApiService

        public async Task<AuthResponse?> GetAuthResponse(LoginRequest request)
        {
            var url = "https://localhost:7137/api/Auth/login";

            var result = await httpClient.PostAsJsonAsync(url, request);

            AuthResponse? response = await result.Content.ReadFromJsonAsync<AuthResponse>();

            if (response?.Token != null)
            {
                // save token in cookie (minimal)
                //_accessor.HttpContext?.Response.Cookies.Append("AuthToken", response.Token, new CookieOptions { HttpOnly = true, Secure = true });
                _accessor.HttpContext?.Session.SetString("AuthToken", response.Token);
            }

            return response;
        }

        // minimal register
        public async Task<bool> RegisterAsync(Models.RegisterRequest request)
        {
            var url = "https://localhost:7137/api/Auth/register";
            var result = await httpClient.PostAsJsonAsync(url, request);
            return result.IsSuccessStatusCode;
        }

        // get token from session

        public string? GetTokenFromSession()
        {
            return _accessor.HttpContext?.Session.GetString("AuthToken");
        }


    }
}
