using System.ComponentModel.DataAnnotations;

namespace Notes_Management_system.Model;

public class User
{
   public string UserId { get; set; } = Guid.NewGuid().ToString();
   public string Name { get; set; }
   public string Email { get; set; }
   public DateTime DateOfBirth { get; set; }
   public string PasswordHash { get; set; }  // Store hashed password
   
}

