using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AstroTrack.UI.ViewModels;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
namespace AstroTrack.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Set the DataContext to the MainViewModel
        if (DataContext is EventsViewModel vm)
        {
            // Set the default selected body to the first one in the list
            _ = vm.LoadEventsAsync();
        }
    }
}