using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(); // Add MVC Controllers & Views
builder.Services.AddHttpClient(); // Enable HttpClient
builder.Services.AddSession(); // Enable Session storage

// Add MVC Controller configuration with Newtonsoft JSON serializer settings
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // Format JSON with C# model
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // Resolve camelCase vs PascalCase
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // Ignore null values
});

// Uncomment and configure HttpClient for your API base URL
 /*var baseUrl = "https://localhost:7268/";
 builder.Services.AddHttpClient("EmployeeAPIBase", client =>
 {
    client.BaseAddress = new Uri(baseUrl);
 });*/

// Add CORS policy to allow any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login"; // Redirect if not logged in
    });
var app = builder.Build();

// Apply CORS policy globally
app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route configuration (controller, action, and optional id)

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Authentication}/{action=Login}/{id?}");

// Map Razor pages (if any)
app.MapRazorPages();
// Start the application
app.Run();
