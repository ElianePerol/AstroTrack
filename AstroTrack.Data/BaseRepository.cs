using SQLite;


/// <summary>
/// Base repository implementing common database operations.
/// </summary>
public abstract class BaseRepository
{
    protected SQLiteConnection Database { get; }


    protected BaseRepository(string dbPath = "astrot­rack.db")
    {
        Database = new SQLiteConnection(dbPath);
    }

    protected void CreateTable<T>() where T : new()
    {
        Database.CreateTable<T>();
    }
}