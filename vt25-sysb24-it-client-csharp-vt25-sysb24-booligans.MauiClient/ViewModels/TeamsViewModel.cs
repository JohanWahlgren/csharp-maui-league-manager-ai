using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels
{
    public class TeamsViewModel : INotifyPropertyChanged
    {
        // ==========
        // Fields
        // ==========
        private readonly ITeamService _teamService; // Readonly means it can only be set once, and in the constructor
        private ObservableCollection<Team> _teams;
        private ObservableCollection<Team> _allTeams; // Stores all teams for filtering, "master list", only used internally for filtering logic.
        private bool _isLoading;
        private string _searchText;
        private bool _hasPerformedSearch;

        // ==========
        // Commands (bound to UI elements), properties that holds actions the UI can trigger
        // ==========
        public ICommand NavigateBackCommand { get; }
        public ICommand SearchCommand { get; }

        // ==========
        // Properties
        // ==========
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value) // Only update if the value has changed
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilter(); // Triggers filtering on every keystroke
                }
            }
        }

        public ObservableCollection<Team> Teams
        {
            get => _teams;
            set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool HasPerformedSearch
        {
            get => _hasPerformedSearch;
            private set
            {
                _hasPerformedSearch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowNoResultsMessage));
            }
        }

        public bool ShowNoResultsMessage => HasPerformedSearch && !IsLoading && Teams.Count == 0; 
        public bool NoTeamsAvailable => !IsLoading && !ShowNoResultsMessage && (Teams == null || Teams.Count == 0);

        // ==========
        // Constructor
        // ==========
        public TeamsViewModel(ITeamService teamService)
        {
            _teamService = teamService; // Stores service in private readonly field _teamService
            _allTeams = new ObservableCollection<Team>(); // Internal list of all teams
            Teams = new ObservableCollection<Team>(); // Property that is bound to the UI's CollectionView

            // Initialize Commands
            NavigateBackCommand = new Command(async () => await Shell.Current.GoToAsync("..")); // Navigate back to previous page
            SearchCommand = new Command<string>(search => SearchText = search); // Takes input string search and sets property SearchText = search

            // Load teams when ViewModel is created
            MainThread.BeginInvokeOnMainThread(async () => await LoadTeamsAsync());
        }

        // ==========
        // Data Loading (engine behind loading and showing team data)
        // ==========
        public async Task LoadTeamsAsync()
        {
            HasPerformedSearch = false; // When loading data, reset search state

            if (IsLoading) // Prevents multiple load requests
                return;

            try
            {
                IsLoading = true;

                var teams = await _teamService.GetTeamsAsync(); // Fetch all teams from service, which fetches from API
                _allTeams.Clear();
                Teams.Clear();

                foreach (var team in teams) // Populate internal list and UI list
                {
                    _allTeams.Add(team);
                    Teams.Add(team);
                }

                // Reapply filter if search exists
                if (!string.IsNullOrWhiteSpace(SearchText))
                    ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading teams: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load teams: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ==========
        // Filtering
        // ==========
        private void ApplyFilter()
        {
            HasPerformedSearch = !string.IsNullOrWhiteSpace(SearchText); // Sets to true if something is written in search box

            Teams.Clear();

            if (!HasPerformedSearch) // If no search, show all teams
            {
                foreach (var team in _allTeams)
                    Teams.Add(team);
                return;
            }

            var filteredTeams = _allTeams // Apply filter to internal list
                .Where(t => t.TeamNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) // Case-insensitive search
                .ToList();

            foreach (var team in filteredTeams) // Add filtered results to UI list
                Teams.Add(team);

            OnPropertyChanged(nameof(ShowNoResultsMessage)); // Notify UI to show "No results" message if no teams are found
        }

        // ==========
        // INotifyPropertyChanged
        // ==========
        public event PropertyChangedEventHandler PropertyChanged; // Event that is triggered when a property changes. 
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => // CallerMemberName = automatically fills in the name of the property that called the method. null allows for passing of name manually.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Invokes the PropertyChanged event, passing this object (the ViewModel) and the name of the property that changed.
    }
}