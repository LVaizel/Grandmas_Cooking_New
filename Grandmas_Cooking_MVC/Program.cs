using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/LoginPage"; // Where to send users who aren't logged in
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120); // How long the login lasts
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register IHttpContextAccessor so services can access HttpContext/Session
builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<Grandmas_Cooking_MVC.InfrastructureLayer.RecipeAPIService>();
//builder.Services.AddScoped<Grandmas_Cooking_MVC.InfrastructureLayer.AuthApiService>();

builder.Services.AddHttpClient<Grandmas_Cooking_MVC.InfrastructureLayer.RecipeAPIService>();
builder.Services.AddHttpClient<Grandmas_Cooking_MVC.InfrastructureLayer.AuthApiService>();


var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LoginPage}/{id?}")
    .WithStaticAssets();


app.Run();
