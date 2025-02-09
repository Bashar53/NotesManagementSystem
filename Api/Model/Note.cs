using System.ComponentModel.DataAnnotations;

namespace Notes_Management_system.Model;
public class Note
{   
    public string NoteId { get; set; } = Guid.NewGuid().ToString(); 
    public string UserEmail { get; set; } 
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Text { get; set; }    
    public NoteType Type { get; set; } 

    public DateTime? ReminderDateTime { get; set; } 
    public DateTime? DueDateTime { get; set; } 
    public bool? IsTaskCompleted { get; set; } 
    public string? BookmarkUrl { get; set; } 

    public bool IsValid() => !string.IsNullOrEmpty(Text) && Text.Length <= 100;
}

public enum NoteType
{
    Regular,   
    Reminder,  
    Task,     
    Bookmark  
}
