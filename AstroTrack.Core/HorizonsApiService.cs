using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Text.Json;

namespace AstroTrack.Core
{
    public class HorizonsApiService
    {
        private readonly HttpClient _http;

        public HorizonsApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://ssd.jpl.nasa.gov/api/")
            };
        }

        /// <summary>
        /// Fetches all close‐approach (Asteroids) events (EPHEM_TYPE=APPROACH) for the given body relative to Earth ("500@399")
        /// </summary>
        public async Task<HorizonsApproachResponse> GetApproachEventsAsync(
            string bodyCommand,
            DateTime from,
            DateTime to,
            string stepSize = "1 d")
        {
            // Build the APPROACH query 
            var qs = $"horizons.api?format=json"
                   + $"&COMMAND='{Uri.EscapeDataString(bodyCommand)}'"
                   + $"&EPHEM_TYPE='APPROACH'"
                   + $"&CENTER='500@399'"
                   + $"&START_TIME='{from:yyyy-MM-dd}'"
                   + $"&STOP_TIME='{to:yyyy-MM-dd}'"
                   + $"&STEP_SIZE='{Uri.EscapeDataString(stepSize)}'"
                   + "&OBJ_DATA='YES'&MAKE_EPHEM='YES'";

            // Use GetFromJsonAsync
            var resp = await _http.GetFromJsonAsync<HorizonsApproachResponse>(qs);

            // If we got nothing or missing Data/Table, return empty
            if (resp?.Data?.Table == null)
            {
                return new HorizonsApproachResponse
                {
                    Data = new HorizonsApproachData
                    {
                        Table = new ApproachTable
                        {
                            Header = Array.Empty<string>(),
                            Rows = Array.Empty<string[]>()
                        }
                    }
                };
            }

            // If the service reported an error string, check if it's one we can ignore:
            if (!string.IsNullOrEmpty(resp?.Error))
            {
                var err = resp.Error.ToUpperInvariant();

                // Swallow all the “no approach for non‑asteroids” errors,
                // any “out of bounds” DXREAD errors, or anything that says “no close-approach”
                if (err.Contains("NON-ASTEROIDS")
                 || err.Contains("DXREAD")
                 || err.Contains("NO EPHEMERIS")
                 || err.Contains("CLOSE-APPROACH TABLES NOT ALLOWED"))
                {
                    // Turn it into an empty table
                    resp.Data = new HorizonsApproachData
                    {
                        Table = new ApproachTable
                        {
                            Header = Array.Empty<string>(),
                            Rows = Array.Empty<string[]>()
                        }
                    };
                    return resp;
                }

                // Otherwise, propagate real errors
                throw new InvalidOperationException($"Horizons error: {resp.Error}");
            }

            return resp;
        }

        /// <summary>
        /// Fetches daily observer ephemeris (EPHEM_TYPE=OBSERVER) for the given body.
        /// </summary>
        public async Task<HorizonsApproachResponse> GetObserverEphemerisAsync(
            string bodyCommand,
            DateTime from,
            DateTime to,
            string stepSize = "1 d",
            string quantities = "1,20,19")
        {
            // Build the OBSERVER query
            var qs = $"horizons.api?format=json"
                   + $"&COMMAND='{Uri.EscapeDataString(bodyCommand)}'"
                   + $"&EPHEM_TYPE='OBSERVER'"
                   + $"&CENTER='500@399'"
                   + $"&START_TIME='{from:yyyy-MM-dd}'"
                   + $"&STOP_TIME='{to:yyyy-MM-dd}'"
                   + $"&STEP_SIZE='{Uri.EscapeDataString(stepSize)}'"
                   + $"&QUANTITIES='{Uri.EscapeDataString(quantities)}'"
                   + "&OBJ_DATA='YES'&MAKE_EPHEM='YES'";

            var resp = await _http.GetFromJsonAsync<HorizonsApproachResponse>(qs);

            if (resp?.Data?.Table == null)
            {
                return new HorizonsApproachResponse
                {
                    Data = new HorizonsApproachData
                    {
                        Table = new ApproachTable
                        {
                            Header = Array.Empty<string>(),
                            Rows = Array.Empty<string[]>()
                        }
                    }
                };
            }

            // If the service reported an error string, check if it's one we can ignore:
            if (!string.IsNullOrEmpty(resp?.Error))
            {
                var err = resp.Error.ToUpperInvariant();

                // Swallow all the “no approach for non‑asteroids” errors,
                // any “out of bounds” DXREAD errors, or anything that says “no close-approach”
                if (err.Contains("NON-ASTEROIDS")
                 || err.Contains("DXREAD")
                 || err.Contains("NO EPHEMERIS")
                 || err.Contains("CLOSE-APPROACH TABLES NOT ALLOWED"))
                {
                    // Turn it into an empty table
                    resp.Data = new HorizonsApproachData
                    {
                        Table = new ApproachTable
                        {
                            Header = Array.Empty<string>(),
                            Rows = Array.Empty<string[]>()
                        }
                    };
                    return resp;
                }

                // Otherwise, propagate real errors
                throw new InvalidOperationException($"Horizons error: {resp.Error}");
            }

            return resp;
        }

        // Response classes for the Horizons API
        public class HorizonsApproachResponse
        {
            [JsonPropertyName("signature")] public object Signature { get; set; }
            [JsonPropertyName("result")] public string Result { get; set; }
            [JsonPropertyName("data")] public HorizonsApproachData Data { get; set; }
            [JsonPropertyName("error")] public string? Error { get; set; }
        }

        public class HorizonsApproachData
        {
            [JsonPropertyName("table")] public ApproachTable Table { get; set; }
        }

        public class ApproachTable
        {
            [JsonPropertyName("header")] public string[] Header { get; set; }
            [JsonPropertyName("rows")] public string[][] Rows { get; set; }
        }
    }
}
