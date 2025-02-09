using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes_Management_system.Model;
using Notes_Management_system.Services;

namespace Notes_Management_system.Controllers;

//[Authorize] // authorize
[Route("api/[controller]")]
[ApiController]

public class NotesController : ControllerBase
{
    private static FileStorageService storage = new FileStorageService();
    [Authorize]
    [HttpPost]
    public IActionResult CreateNote([FromBody] Note note)
    {

      
        var notes = storage.LoadNotes();
        notes.Add(note);
        storage.SaveNotes(notes);
        return Ok(note);
    }
    private static HashSet<string> revokedTokens = new HashSet<string>();
   [Authorize]
    [HttpGet]
    public IActionResult GetNotes()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
         if (!string.IsNullOrEmpty(token))
         {
             revokedTokens.Add(token);
         }
        
        
        var notes = storage.LoadNotes();
        if (notes == null || notes.Count == 0)
        {
            return Ok(new List<Note>()); // ⬅ Ensures API returns `[]` instead of `null`
        }

        return Ok(notes);
    }
    public static bool IsTokenRevoked(string token)
    {
        return revokedTokens.Contains(token);
    }
}
