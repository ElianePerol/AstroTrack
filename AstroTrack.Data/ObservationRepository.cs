using SQLite;
using AstroTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Repository for managing Observation data operations.
/// </summary>
public class ObservationRepository : BaseRepository
{
    public ObservationRepository(string dbPath = "astrotrack.db") : base(dbPath)
    {
        CreateTable<Observation>();
    }

    public List<Observation> GetAll()
    {
        return Database.Table<Observation>().ToList();
    }

    public Observation GetById(int id)
    {
        return Database.Get<Observation>(id);
    }

    public List<Observation> GetByUser(int userId)
    {
        return Database.Table<Observation>().Where(o => o.UserId == userId).ToList();
    }

    public List<Observation> GetByEvent(int eventId)
    {
        return Database.Table<Observation>().Where(o => o.EventId == eventId).ToList();
    }

    public int Save(Observation observation)
    {
        if (observation.Id != 0)
            return Database.Update(observation);
        else
            return Database.Insert(observation);
    }

    public int Delete(int id)
    {
        return Database.Delete<Observation>(id);
    }

    public void PopulateTestData()
    {
        var list = new List<Observation>
        {
            new Observation { UserId = 1, EventId = 1,
                              DateTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                              Notes = "Superbe!", Rating = 5 },
            new Observation
            {
                UserId = 1,
                EventId = 2,
                DateTimestamp = DateTimeOffset.Now.AddDays(-2)
                                                             .ToUnixTimeSeconds(),
                Notes = "Nuageux…",
                Rating = 3
            }
        };
    foreach (var o in list)
        Database.Insert(o);
    }

}