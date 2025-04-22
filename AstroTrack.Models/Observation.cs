using SQLite;

/// <summary>
/// Represents a user's observation of an astronomical event.
/// Links users to the events they've observed.
/// </summary>

namespace AstroTrack.Models 
{ 
    public class Observation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public long DateTimestamp { get; set; }
        public string Notes { get; set; }
        public int Rating { get; set; }

    }
}
