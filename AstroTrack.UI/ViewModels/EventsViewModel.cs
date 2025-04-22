using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AstroTrack.Core;
using AstroTrack.Models;

namespace AstroTrack.UI.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Event> Events { get; }

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set { _selectedEvent = value; OnPropertyChanged(); }
        }

        public EventsViewModel()
        {
            // Load events from the data service
            var list = DataService.Instance.Events.GetAll();
            Events = new ObservableCollection<Event>(list);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
