// John, Gurvansh - AuthApiService.cs created for handling authentication API calls and user session management.

using Grandmas_Cooking_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Net.Http.Json;
using System.Text.Json;

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

            HttpResponseMessage result;
            try
            {
                result = await httpClient.PostAsJsonAsync(url, request);
            }
            catch (Exception)
            {
                // network/connection error
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            AuthResponse? response = null;
            try
            {
                response = JsonSerializer.Deserialize<AuthResponse?>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                // Response wasn't valid JSON.
                return null;
            }

            if (response?.Token != null)
            {
                //Create User Claims (Store the JWT token inside the user's cookie data)
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, request.UserName ?? "User"),
                        new Claim("RecipeApiToken", response.Token)
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                // Sign in only if we have an HttpContext (avoid NRE in background scenarios)
                if (_accessor.HttpContext != null)
                {
                    await _accessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }
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
            return _accessor.HttpContext?.User.FindFirst("RecipeApiToken")?.Value;
        }
    }
}
