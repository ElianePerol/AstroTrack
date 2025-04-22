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
            InitializeDatabase();

            var now = DateTime.UtcNow;
            var toDate = now.AddDays(30);

            var horizons = new HorizonsRepository(_dbPath);

            // 1) Asteroid close-approaches
            var asteroidIds = new[]
            {
                "2000001", // Ceres (#1)
                "2000002", // Pallas (#2)
                "2000003", // Juno   (#3)
                "2000004", // Vesta  (#4)
                "2000433",  // Eros   (#433) }
            };
            foreach (var id in asteroidIds)
            {
                Console.WriteLine($"Syncing approaches for asteroid {id}...");
                await horizons.SyncApproachesAsync(id, now, toDate);
            }

            // 2) Planet & Moon ephemeris
            var planetIds = new[] { "-198", "10", "499", "599", "699", "799" };
            foreach (var id in planetIds)
            {
                Console.WriteLine($"Syncing ephemeris for body {id}...");
                await horizons.SyncObserverEphemerisAsync(id, now, toDate);
            }

            Console.WriteLine("All data synced.");
        }

        // Closes the database connection.
        public void Close()
        {
            _database?.Close();
            _database = null;
        }
    }

}