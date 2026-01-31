using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels
{
    public class MainViewModel
    {
        public ICommand NavigateToTeamsCommand { get; }
        public ICommand NavigateToPlayersCommand { get; }
        public ICommand NavigateToTeamsReportsCommand { get; }

        public MainViewModel() // When the MainViewModel is created, the NavigateToTeamsCommand, NavigateToPlayersCommand and NavigateToTeamsReportsCommand are created
        {
            NavigateToTeamsCommand = new Command(async () =>
                await Shell.Current.GoToAsync("teams"));

            NavigateToPlayersCommand = new Command(async () =>
                await Shell.Current.GoToAsync("players"));

            NavigateToTeamsReportsCommand = new Command(async () => 
                await Shell.Current.GoToAsync("teamsreport"));
        }
    }
}