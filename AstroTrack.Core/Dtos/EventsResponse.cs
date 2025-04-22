using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Namespace for DTOs related to AstronomyAPI.com.
/// </summary>
namespace AstroTrack.Core.Dtos
{
    // Top-level response wrapper for body events from AstronomyAPI.com.
    public class EventsResponse
    {
        [JsonPropertyName("data")]
        public EventsData Data { get; set; }
    }

    // Container for rows of event data.
    public class EventsData
    {
        [JsonPropertyName("rows")]
        public List<EventRow> Rows { get; set; }
    }

    // Represents a single event entry in the response, containing a body and its events.
    public class EventRow
    {
        [JsonPropertyName("body")]
        public BodyInfo Body { get; set; }

        [JsonPropertyName("events")]
        public List<SubEvent> Events { get; set; }
    }

    // Represents one event entry under a row, with type and date.
    public class SubEvent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("date")]
        public DateInfo Date { get; set; }

        // Add additional fields as needed
    }

    // Contains basic information about the celestial body.
    public class BodyInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // Contains date information in ISO format.
    public class DateInfo
    {
        [JsonPropertyName("iso")]
        public string Iso { get; set; }
    }
}
