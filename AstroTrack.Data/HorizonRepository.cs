using System;
using System.Globalization;
using System.Threading.Tasks;
using AstroTrack.Core;
using AstroTrack.Models;
using AstroTrack.Common;

namespace AstroTrack.Data
{
    /// <summary>
    /// Repository to sync NASA JPL Horizons close-approach events into local SQLite database.
    /// </summary>
    public class HorizonsRepository
    {
        private readonly HorizonsApiService _api;
        private readonly EventRepository _evtRepo;

        public HorizonsRepository(string dbPath)
        {
            _api = new HorizonsApiService();
            _evtRepo = new EventRepository(dbPath);
        }

        // Syncs asteroid close‐approach events (APPROACH).
        public async Task SyncApproachesAsync(
            string naifId,
            DateTime from,
            DateTime to,
            string stepSize = "1 d")
        {
            // Fetch the ephemeris data
            var response = await _api.GetApproachEventsAsync(naifId, from, to, stepSize);
            var header = response.Data.Table.Header;
            var rows = response.Data.Table.Rows;

            // Find the index of the columns we need
            int idxDate = Array.IndexOf(header, "Date__(UT)__HR:MN");
            int idxDist = Array.IndexOf(header, "r (AU)");

            foreach (var row in rows)
            {
                if (!DateTime.TryParseExact(
                        row[idxDate],
                        "yyyy-MMM-dd HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal,
                        out var dt))
                    continue;

                if (!double.TryParse(
                        row[idxDist],
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var distAu))
                    distAu = 0;

                _evtRepo.Save(new Event
                {
                    BodyId = naifId,
                    Name = $"Close approach of {naifId}",
                    DateTimestamp = DateTimeHelper.ToUnixTimestamp(dt),
                    Type = "Approach",
                    Description = $"Distance: {distAu:F6} AU"
                });
            }
        }

        // Syncs daily observer ephemeris (OBSERVER).
        public async Task SyncObserverEphemerisAsync(
            string naifId,
            DateTime from,
            DateTime to,
            string stepSize = "1 d")
        {
            // Fetch the ephemeris data
            var response = await _api.GetObserverEphemerisAsync(naifId, from, to, stepSize);
            var header = response.Data.Table.Header;
            var rows = response.Data.Table.Rows;

            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                if (!DateTime.TryParse(row[0], out var dt)) continue;

                var ra = row.Length > 1 ? row[1] : "";
                var dec = row.Length > 2 ? row[2] : "";

                _evtRepo.Save(new Event
                {
                    BodyId = naifId,
                    Name = $"{naifId} Ephemeris",
                    DateTimestamp = DateTimeHelper.ToUnixTimestamp(dt),
                    Type = "Observer",
                    Description = $"RA: {ra}, Dec: {dec}"
                });
            }
        }
    }
}
