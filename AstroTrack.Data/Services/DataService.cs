using SQLite;
using System.IO;
using System;
using System.Threading.Tasks;
using AstroTrack.Models;
using AstroTrack.Data;

namespace AstroTrack.Data
{
    /// <summary>
    /// Core data service providing database access.
    /// Implements the Singleton pattern for global access.
    /// </summary>
    public class DataService : IDataService
    {
        private static DataService _instance;
        private SQLiteConnection _database;
        private readonly string _dbPath;

        // Gets the singleton instance of the DataService.
        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataService();
                return _instance;
            }
        }

        // Path to the SQLite database file.
        public string DbPath => _dbPath;

        // Repositories
        public EventRepository Events { get; private set; }
        public UserRepository Users { get; private set; }
        public ObservationRepository Observations { get; private set; }


        // Private constructor to prevent direct instantiation, initialises db path
        private DataService()
        {
            // Set the database path to the ApplicationData folder
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _dbPath = Path.Combine(folder, "AstroTrack.db");
            Console.WriteLine($"Database located at: {_dbPath}");
        }

        // Synchronous initialize method for compatibility with IDataService.
        public void Initialize()
        {
            // Initialize database and tables only (no API sync here)
            InitializeDatabase();
        }

        // Initializes the database connection and repositories.
        public void InitializeDatabase()
        {
            _database = new SQLiteConnection(_dbPath);

            // Initialize repositories
            Events = new EventRepository(_dbPath);
            Users = new UserRepository(_dbPath);
            Observations = new ObservationRepository(_dbPath);

            Console.WriteLine("Database initialized successfully");

            // Add test data if database is empty
            //if (_database.Table<Event>().Count() == 0)
            //{
            //    Users.PopulateTestData();
            //    Events.PopulateTestData();
            //    Observations.PopulateTestData();
            //}
        }

        // Asynchronously initializes database and preloads events for all bodies.
        public async Task InitializeAsync(double latitude, double longitude, double elevation)
        {
            try
            {
                // Setup DB and tables
                InitializeDatabase();

                // List of bodies to fetch
                var bodies = new[] { "moon", "sun", "mercury", "venus", "mars", "jupiter", "saturn" };
                var now = DateTime.UtcNow;
                var toDate = now.AddDays(30);

                var repo = new AstronomyRepository(_dbPath);
                // Fetch each body's events and save locally
                foreach (var bodyId in bodies)
                {
                    Console.WriteLine($"Syncing events for {bodyId}...");
                    try
                    {
                        await repo.SyncBodyEventsAsync(bodyId, latitude, longitude, elevation, now, toDate);
                    }
                    catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // 404 means this body simply has no events endpoint—skip it
                        Console.WriteLine($"No events endpoint for '{bodyId}', skipping.");
                    }
                    catch (Exception ex)
                    {
                        // Any other error we want to see at startup, so rethrow
                        Console.Error.WriteLine($"Error syncing '{bodyId}': {ex.Message}");
                        throw;
                    }
                }
                Console.WriteLine("All body events synced.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Data synchronization failed: {ex.Message}");
                throw;
            }
        }

        // Closes the database connection.
        public void Close()
        {
            _database?.Close();
            _database = null;
        }
    }

}