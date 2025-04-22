using SQLite;
using System.IO;
using System;
using AstroTrack.Models;
using AstroTrack.Data;

namespace AstroTrack.Core
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

        // Repositories
        public EventRepository Events { get; private set; }
        public UserRepository Users { get; private set; }
        public ObservationRepository Observations { get; private set; }

        /// <summary>
        /// Gets the singleton instance of the DataService.
        /// </summary>
        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataService();
                return _instance;
            }
        }

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        private DataService()
        {
            // Set the database path to the ApplicationData folder
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _dbPath = Path.Combine(folder, "AstroTrack.db");
            Console.WriteLine($"Database located at: {_dbPath}");


            Initialize();
        }

        /// <summary>
        /// Initializes the database connection and repositories.
        /// </summary>
        public void Initialize()
        {
            try
            {
                _database = new SQLiteConnection(_dbPath);

                // Initialize repositories
                Events = new EventRepository(_dbPath);
                Users = new UserRepository(_dbPath);
                Observations = new ObservationRepository(_dbPath);

                Console.WriteLine("Database initialized successfully");

                // Add test data if database is empty
                if (_database.Table<Event>().Count() == 0)
                {
                    Users.PopulateTestData();
                    Events.PopulateTestData();
                    Observations.PopulateTestData();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes the database connection.
        /// </summary>
        public void Close()
        {
            _database?.Close();
            _database = null;
        }
    }

}