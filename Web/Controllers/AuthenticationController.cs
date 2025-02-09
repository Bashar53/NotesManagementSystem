using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;

using FrontEnd.Controllers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FrontEnd.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient _httpClient;

        //public AuthenticationController(IHttpClientFactory httpClient) => _httpClient = httpClient.CreateClient("EmployeeAPIBase");
      
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] UserVM request)
        {
            
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                ViewData["ErrorMessage"] = "Validation failed: " + string.Join(", ", errors);
                return View(request);
            }
            var apiUrl = "https://localhost:7237/api/Authentication/login";

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

                // Store JWT Token in session
                HttpContext.Session.SetString("jwtToken", result.Token);

                return RedirectToAction("Dashboard", "Index"); // Redirect after login
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid login credentials";
                return View();
            }
        }


        [HttpGet]
        public IActionResult Signup()   
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Signup(UserVM user)
        {
            if (ModelState.IsValid)
            {
                // Serialize the user object into JSON
                var userJson = JsonConvert.SerializeObject(user);
                var content = new StringContent(userJson, Encoding.UTF8, "application/json");

                // Send HTTP POST request to the API
                var response = await _httpClient.PostAsync("https://localhost:7237/api/Authentication/signup", content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    TempData["SuccessMessage"] = "User registered successfully!";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Handle failure
                    TempData["ErrorMessage"] = "Failed to register user. Please try again.";
                    return View();
                }
            }

                return View(user);
            
        }




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Get the JWT token from the client's browser (localStorage or cookie)
            var token = Request.Cookies["jwtToken"]; // Or fetch it from localStorage if using JS

            if (string.IsNullOrEmpty(token))
            {
                // If no token is found, the user is already logged out or the session expired
                return RedirectToAction("Login", "Authentication");
            }

            // Create the HTTP request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7237/api/Authentication/logout")
            {
                Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", token)
            }
            };

            // Send the request to the API
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Log out the user from the MVC application by removing the JWT token
                Response.Cookies.Delete("jwtToken");

                // Optionally, show a success message
                TempData["LogoutSuccess"] = "You have logged out successfully.";

                // Redirect to the login page or another page
                return RedirectToAction("Login", "Authentication");
            }
            else
            {
                // Handle failure
                TempData["LogoutError"] = "Logout failed. Please try again later.";

                // Redirect to the dashboard or another page
                return RedirectToAction("Index", "Dashboard");
            }
        }


    }



    // DTOs for Login Request and API Response

    public class UserVM 
     {
         public string Name { get; set; }
         public string Email { get; set; }
         public DateTime? DateOfBirth { get; set; }
         public string PasswordHash { get; set; }  // Store hashed password

     }


     public class AuthResponse
     {
         public string Token { get; set; }
     }
       
    }








