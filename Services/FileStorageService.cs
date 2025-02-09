using Newtonsoft.Json;
using Notes_Management_system.Model;
using System.Text.Json;


namespace Notes_Management_system.Services
{
    public class FileStorageService
    {
        private readonly string NoteFilePath = "notes.json";

        public List<Note> LoadNotes()
        {
            try
            {
                if (!File.Exists(NoteFilePath))
                {
                    Console.WriteLine("notes.json file does not exist.");
                    return new List<Note>();
                }

                string json = File.ReadAllText(NoteFilePath);
                Console.WriteLine("JSON Content Read: " + json); // Debugging

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("notes.json file is empty.");
                    return new List<Note>();
                }

                var notes = System.Text.Json.JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
                Console.WriteLine($"Total Notes Loaded: {notes.Count}"); // Debugging


                if (!File.Exists(NoteFilePath)) return new List<Note>();
                return JsonConvert.DeserializeObject<List<Note>>(File.ReadAllText(NoteFilePath)) ?? new List<Note>();
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine("Error in JSON format: " + ex.Message);
                return new List<Note>();
            }
        }

        public void SaveNotes(List<Note> notes)
        {
            File.WriteAllText(NoteFilePath, JsonConvert.SerializeObject(notes));
        }

        private readonly string userFilePath = "users.json";

        // Load users from file
        public List<User> LoadUsers()
        {
          
                try
                {
                    if (!File.Exists(userFilePath))
                    {
                        Console.WriteLine("users.json file does not exist.");
                        return new List<User>();
                    }

                    string json = File.ReadAllText(userFilePath);
                    Console.WriteLine("JSON Content Read: " + json); // Debugging

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        Console.WriteLine("users.json file is empty.");
                        return new List<User>();
                    }

                    var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                    Console.WriteLine($"Total Users Loaded: {users.Count}"); // Debugging

                    return users;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine("Error in JSON format: " + ex.Message);
                    return new List<User>();
                }
            }




        // Save users to file
        public void SaveUsers(List<User> users)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(userFilePath, json);
                Console.WriteLine("User data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving users: " + ex.Message);
            }
        }



    }

}
