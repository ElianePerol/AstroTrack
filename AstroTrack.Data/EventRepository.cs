using SQLite;
using AstroTrack.Models;
using System.Collections.Generic;
using System;

/// <summary>
/// Repository for managing Event data operations.
/// </summary>
public class EventRepository : BaseRepository
{
    public EventRepository(string dbPath = "astrotrack.db")
        : base(dbPath)
    {
        CreateTable<Event>();
    }

    public List<Event> GetAll()
    {
        return Database.Table<Event>().ToList();
    }

    public Event GetById(int id)
    {
        return Database.Get<Event>(id);
    }

    public int Save(Event evt)
    {
        if (evt.Id != 0)
            return Database.Update(evt);
        else
            return Database.Insert(evt);
    }

    public int Delete(int id)
    {
        return Database.Delete<Event>(id);
    }

    public void PopulateTestData()
    {
        var events = new List<Event>
        {
            new Event
            {
                Name = "Total Solar Eclipse",
                DateTimestamp = DateTimeOffset.Now.AddDays(30).ToUnixTimeSeconds(),
                Type = "Eclipse",
                Description = "A total solar eclipse visible across Europe",
                PositionX = 1.0,
                PositionY = 0.0,
                PositionZ = 0.0
            },
            new Event
            {
                Name = "Venus-Jupiter Conjunction",
                DateTimestamp = DateTimeOffset.Now.AddDays(15).ToUnixTimeSeconds(),
                Type = "Conjunction",
                Description = "Spectacular conjunction between Venus and Jupiter",
                PositionX = -0.5,
                PositionY = 0.25,
                PositionZ = 0.5
            }
        };

        foreach (var evt in events)
            Database.Insert(evt);
    }
}