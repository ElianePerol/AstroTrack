using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AstroTrack.Common;
using AstroTrack.Models;
using AstroTrack.Core.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using System.Diagnostics;

/// <summary>
/// Class to interact with the Astronomy API (AstronomyAPI.com).
/// </summary>

namespace AstroTrack.Core
{
    public class AstronomyApiService
    {
        private readonly HttpClient _http;

        public AstronomyApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://api.astronomyapi.com/api/v2/")
            };

            // Set up the API key for authentication
            var id = Environment.GetEnvironmentVariable("ASTRO_API_ID");
            var secret = Environment.GetEnvironmentVariable("ASTRO_API_SECRET");

            // Check if the API ID and secret are set
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Missing ASTRO_API_ID or ASTRO_API_SECRET");

            // Log the API ID and secret length for debugging purposes
            Console.WriteLine($"[Debug] Using API ID={id}, Secret length={secret.Length}");

            // Encode the ID and secret for Basic Authentication
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{id}:{secret}"));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", creds);
        }

        // Fetches events (eclipses, conjunctions, etc.) for a given celestial body and time range.
        public async Task<EventsResponse> GetBodyEventsAsync(
            string bodyId,
            double latitude,
            double longitude,
            double elevation,
            DateTime from,
            DateTime to)
        {
            // Format dates as ISO strings (YYYY-MM-DD)
            var fromDate = from.ToString("yyyy-MM-dd");
            var toDate = to.ToString("yyyy-MM-dd");
            var time = from.ToString("HH:mm:ss");    // REQUIRED by the API


            // Build the request URL
            var url = $"bodies/events/{bodyId}"
               + $"?latitude={latitude}"
               + $"&longitude={longitude}"
               + $"&elevation={elevation}"
               + $"&from_date={fromDate}"
               + $"&to_date={toDate}"
               + $"&time={time}"
               + $"&output=rows";

            // manually send GET so we can log status & body
            var msg = await _http.GetAsync(url);
            var content = await msg.Content.ReadAsStringAsync();

            // log for debugging
            Debug.WriteLine($"[API] GET {url}");
            Debug.WriteLine($"[API] Status {(int)msg.StatusCode} {msg.StatusCode}");
            Debug.WriteLine($"[API] Body:\n{content}");

            // make sure it’s 200 OK
            msg.EnsureSuccessStatusCode();

            // parse with System.Text.Json
            var dto = JsonSerializer.Deserialize<EventsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto?.Data?.Rows == null)
                throw new InvalidOperationException("No rows in the API response.");

            return dto;

            // Perform the GET and deserialize
            //var response = await _http.GetFromJsonAsync<EventsResponse>(url);
            //if (response?.Data?.Rows == null)
            //    throw new InvalidOperationException("API returned no event data.");

            //return response;

        }
    }
}
