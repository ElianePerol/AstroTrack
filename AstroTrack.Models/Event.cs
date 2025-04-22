using SQLite;
using AstroTrack.Common;

/// <summary>
/// Represents an astronomical event that can be observed.
/// </summary>

namespace AstroTrack.Models
{
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public long DateTimestamp { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }

        // Uses the DateTimeHelper to convert the Unix timestamp to a DateTime object
        public DateTime Date
        {
            get
            {
                return DateTimeHelper.FromUnixTimestamp(DateTimestamp);
            }
        }
    }

    }
