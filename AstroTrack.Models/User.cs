using SQLite;

/// <summary>
/// Represents a user of the AstroTrack application.
/// </summary>

namespace AstroTrack.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}