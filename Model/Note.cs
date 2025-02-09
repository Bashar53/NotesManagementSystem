namespace Notes_Management_system.Model;
public class Note
{   
    public string NoteId { get; set; } = Guid.NewGuid().ToString(); // Unique ID for each note
    public string UserEmail { get; set; }  // Associate note with a specific user   
    public string Text { get; set; }    // Note content (max 100 characters)
    public NoteType Type { get; set; }  // Type of note 

    // Optional fields for specific note types
    public DateTime? ReminderDateTime { get; set; } // For reminder notes
    public DateTime? DueDateTime { get; set; } // For tasks
    public bool? IsTaskCompleted { get; set; } // Task completion status
    public string? BookmarkUrl { get; set; } // For bookmarks

    // Validate max length (enforce in API)
    public bool IsValid() => !string.IsNullOrEmpty(Text) && Text.Length <= 100;
}

public enum NoteType
{
    Regular,    // Simple text note
    Reminder,   // Note with a reminder (date + time)
    Task,       // Note with due date + completion status
    Bookmark    // Bookmark a URL
}
