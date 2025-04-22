using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using AstroTrack.Core;
using AstroTrack.Data;
using AstroTrack.Models;
using AstroTrack.Core.Dtos;
using AstroTrack.Common;
using AstroTrack.UI.ViewModels;

/// <summary>
/// Class to manage the events view model.
/// </summary>
namespace AstroTrack.UI.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {
        // List bound to DataGrid
        public ObservableCollection<Event> Events { get; } = new ObservableCollection<Event>();


        // List of bodies for the ComboBox
        public ObservableCollection<BodyInfo> AvailableBodies { get; } =
            new ObservableCollection<BodyInfo>
        {
            new BodyInfo { Id = "moon",    Name = "Moon"    },
            new BodyInfo { Id = "sun",     Name = "Sun"     },
            new BodyInfo { Id = "mercury", Name = "Mercury" },
            new BodyInfo { Id = "venus",   Name = "Venus"   },
            new BodyInfo { Id = "mars",    Name = "Mars"    },
            new BodyInfo { Id = "jupiter", Name = "Jupiter" },
            new BodyInfo { Id = "saturn",  Name = "Saturn"  }
        };

        private BodyInfo _selectedBody;
        public BodyInfo SelectedBody
        {
            get => _selectedBody;
            set { _selectedBody = value; OnPropertyChanged(); }
        }

        // Reload events
        public RelayCommand LoadEventsCommand { get; private set; }

        public EventsViewModel()
        {
            // Default to the first body
            SelectedBody = AvailableBodies[0];

            // Wire up the button to call LoadEventsAsync
            LoadEventsCommand = new RelayCommand(async () => await LoadEventsAsync());
        }


        /// Fetches events for the selected body and repopulates the Events collection.
        public async Task LoadEventsAsync()
        {
            // Clear old data
            Events.Clear();

            var list = DataService.Instance.Events
                 .GetAll()
                 .Where(e => e.BodyId == SelectedBody.Id);
            foreach (var e in list)
                Events.Add(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
