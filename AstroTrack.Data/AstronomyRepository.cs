using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AstroTrack.Core;
using AstroTrack.Core.Dtos;
using AstroTrack.Common;
using AstroTrack.Models;

namespace AstroTrack.Data
{
    public class AstronomyRepository
    {
        private readonly EventRepository _eventRepo;
        private readonly AstronomyApiService _api;

        public AstronomyRepository(string dbPath = "astrotrack.db")
        {
            _eventRepo = new EventRepository(dbPath);
            _api = new AstronomyApiService();
        }

        // Fetches events for a specific celestial body from AstronomyAPI.com and persists them locally.
        public async Task SyncBodyEventsAsync(
            string bodyId,
            double latitude,
            double longitude,
            double elevation,
            DateTime from,
            DateTime to)
        {
            // Call the AstronomyAPI.com endpoint for body events
            EventsResponse response = await _api.GetBodyEventsAsync(
                bodyId, latitude, longitude, elevation, from, to);

            // Iterate over each returned event row
            foreach (var row in response.Data.Rows)
            {
                foreach (var evDto in row.Events)
                {
                    DateTime eventDate = DateTime.Parse(evDto.Date.Iso).ToLocalTime();
                    var evt = new Event
                    {
                        Name = $"{evDto.Type} of {row.Body.Name}",
                        DateTimestamp = DateTimeHelper.ToUnixTimestamp(eventDate),
                        Type = evDto.Type,
                        Description = $"Event for {row.Body.Name} on {eventDate:d}"
                    };
                    _eventRepo.Save(evt);
                }
            }
        }
    }
}
