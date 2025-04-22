using SQLite;
using AstroTrack.Models;
using System.Collections.Generic;

/// <summary>
/// Repository for managing User data operations.
/// </summary>
public class UserRepository : BaseRepository
{
    public UserRepository(string dbPath = "astrotrack.db") : base(dbPath)
    {
        CreateTable<User>();
    }

    public List<User> GetAll()
    {
        return Database.Table<User>().ToList();
    }

    public User GetById(int id)
    {
        return Database.Get<User>(id);
    }

    public int Save(User user)
    {
        if (user.Id != 0)
            return Database.Update(user);
        else
            return Database.Insert(user);
    }

    public int Delete(int id)
    {
        return Database.Delete<User>(id);
    }

    public void PopulateTestData()
    {
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };
        Database.Insert(user);
    }
}