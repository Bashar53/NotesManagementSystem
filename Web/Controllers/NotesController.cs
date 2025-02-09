using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontEnd.Controllers
{
    public class NotesController : Controller
    {
        private readonly HttpClient _httpClient;

        // Constructor: Inject HttpClient
        public NotesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NoteVM()); // Ensure model is passed
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteVM note)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Validation failed.";
                return View(note);
            }

            // Initialize missing values
            

            var apiUrl = "https://localhost:7237/api/Notes/CreateNote";

            var jsonContent = JsonSerializer.Serialize(note);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ViewData["ErrorMessage"] = "Failed to create note.";
                return View(note);
            }
        }   
    }

    public class NoteVM
    {
        public string NoteId { get; set; } 
        public string UserEmail { get; set; } 
        public string Text { get; set; } 
        public NoteType Type { get; set; } 
        public DateTime? ReminderDateTime { get; set; }
        public DateTime? DueDateTime { get; set; }
        public bool? IsTaskCompleted { get; set; } = false;
        public string? BookmarkUrl { get; set; } = string.Empty;

        public bool IsValid() => !string.IsNullOrEmpty(Text) && Text.Length <= 100;
    }

    public enum NoteType
    {
        Regular,
        Reminder,
        Task,
        Bookmark
    }
}
