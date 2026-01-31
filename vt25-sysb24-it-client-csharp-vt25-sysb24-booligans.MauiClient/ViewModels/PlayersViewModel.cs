using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;
using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels
{
    public class PlayersViewModel : INotifyPropertyChanged
    {
        // ============
        // Fields (like instance variables)
        // ============
        private readonly IPlayerService _playerService; // readonly means it can only be set once, after it can't be changed. Assigned in the constructor, don't want to ever change it.
        private ObservableCollection<PlayerGroup> _playerGroups; // field that stores the data from the property.
        private List<Player> _allPlayers;
        private bool _isLoading;
        private string _searchText;
        private bool _isGrouped = true;
        private SortingOption _currentSortOption = SortingOption.Team;
        private bool _hasPerformedSearch;

        // Sorting button colors
        private Color _teamSortButtonColor = Colors.Blue; // predefined color built into MAUI
        private Color _lastNameSortButtonColor = Color.FromArgb("#cccccc"); // custom color from a hex value
        private Color _firstNameSortButtonColor = Color.FromArgb("#cccccc");
        private Color _numberSortButtonColor = Color.FromArgb("#cccccc");

        // ============
        // Enum
        // ============
        public enum SortingOption // custom enum to define the sorting options
        {
            Team,
            LastName,
            FirstName,
            Number
        }

        // ============
        // Commands (read-only properties), (bound to UI elements for handling sorting, searching, navigation, and data loading.)
        // ============
        public ICommand NavigateBackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SortByTeamCommand { get; }
        public ICommand SortByLastNameCommand { get; }
        public ICommand SortByFirstNameCommand { get; }
        public ICommand SortByNumberCommand { get; }

        // ============
        // Properties
        // ============
        public ObservableCollection<PlayerGroup> PlayerGroups // The list of player groups shown in the app; updates the UI and visibility of "no players" messages when changed.
        {
            get => _playerGroups;
            set
            {
                _playerGroups = value; // New data being assigned to the property
                OnPropertyChanged(); // Notify the UI that the property has changed
                OnPropertyChanged(nameof(NoPlayersAvailable)); // These three depend on the contents of this list
                OnPropertyChanged(nameof(ShowNoSearchResults));
            }
        }

        public string SearchText // Bound to the SearchBar; updates when user types and triggers player filtering.
        {
            get => _searchText;
            set
            {
                if (_searchText != value) // Checks if the new value is different from the current value
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public bool IsLoading // Indicates whether data is currently loading; used to show/hide the loading spinner in the UI.
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsGrouped  // Indicates whether players are currently grouped by team (true), or shown in a single list (false) sorted by last/first name etc. 
        {
            get => _isGrouped;
            private set // private: Can only be set from within the class
            {
                if (_isGrouped != value)
                {
                    _isGrouped = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasPerformedSearch // Indicates whether the user has performed a search; used to show/hide "no search results" message in the UI.
        {
            get => _hasPerformedSearch;
            private set
            {
                if (_hasPerformedSearch != value)
                {
                    _hasPerformedSearch = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowNoSearchResults));
                }
            }
        }

        public bool NoPlayersAvailable => // True if not loading and player list empty
            !IsLoading &&
            (PlayerGroups == null || PlayerGroups.Count == 0 || PlayerGroups.All(g => g.Count == 0));

        public bool ShowNoSearchResults => HasPerformedSearch && NoPlayersAvailable; // True if search performed and no players available. This property depends on NoPlayersAvailable and HasPerformedSearch.

        // Sorting Button Colors
        public Color TeamSortButtonColor // Colors for sorting buttons; UI updates to highlight the active sort option.
        {
            get => _teamSortButtonColor;
            set { _teamSortButtonColor = value; OnPropertyChanged(); }
        }

        public Color LastNameSortButtonColor
        {
            get => _lastNameSortButtonColor;
            set { _lastNameSortButtonColor = value; OnPropertyChanged(); }
        }

        public Color FirstNameSortButtonColor
        {
            get => _firstNameSortButtonColor;
            set { _firstNameSortButtonColor = value; OnPropertyChanged(); }
        }

        public Color NumberSortButtonColor
        {
            get => _numberSortButtonColor;
            set { _numberSortButtonColor = value; OnPropertyChanged(); }
        }

        // ============
        // Constructor (Called automatically via Dependency Injection; sets up commands, initializes data, and loads players on startup.)
        // ============
        public PlayersViewModel(IPlayerService playerService) 
        {
            _playerService = playerService;
            _allPlayers = new List<Player>();
            PlayerGroups = new ObservableCollection<PlayerGroup>();

            // Initialize Commands
            NavigateBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            SearchCommand = new Command<string>(search => SearchText = search);
            SortByTeamCommand = new Command(() => ApplySorting(SortingOption.Team));
            SortByLastNameCommand = new Command(() => ApplySorting(SortingOption.LastName));
            SortByFirstNameCommand = new Command(() => ApplySorting(SortingOption.FirstName));
            SortByNumberCommand = new Command(() => ApplySorting(SortingOption.Number));

            // Load players when ViewModel is created
            MainThread.BeginInvokeOnMainThread(async () => await LoadPlayersAsync());
        }

        // ============
        // Data Loading
        // ============
        private async Task LoadPlayersAsync()
        {
            if (IsLoading) return; // Don't load if already loading, prevents multiple load requests

            try
            {
                IsLoading = true;

                var players = await _playerService.GetPlayersAsync(); // Load players from the service

                if (players == null || !players.Any()) // Popup message displayed if nothing is returned
                {
                    await DisplayAlert("No Data Available", "No player data is currently available. Please try again later.");
                    _allPlayers = new List<Player>();
                }
                else
                {
                    _allPlayers = players.ToList();
                    ApplySorting(_currentSortOption); // Sorts by team by default because of the field
                }
            }
            catch (Exception ex) // Error message displayed as popup if an exception occurs
            {
                await DisplayAlert("Error", $"Failed to load players: {ex.Message}");
                _allPlayers = new List<Player>();
                PlayerGroups.Clear();
            }
            finally // Loading spinner is always "turned off"
            {
                IsLoading = false;
            }
        }

        // ============
        // Filtering
        // ============
        private void ApplyFilter() // "What should I see" - filters the player list based on the search text.
        {
            if (string.IsNullOrWhiteSpace(SearchText)) // If search bar is empty, sort by team.
            {
                HasPerformedSearch = false;
                ApplySorting(_currentSortOption);
                return;
            }

            HasPerformedSearch = true;

            var filteredPlayers = _allPlayers // Filters _allPlayers to include those whose PlayerNo contains the search text.
                .Where(p => p.PlayerNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) // Search is case-insensitive, Contains() doesnt care where in the string the match happens.
                .ToList();

            PlayerGroups.Clear(); // Clears current displayed player list so filtered players can be added.

            if (filteredPlayers.Count == 0) // If no players match the search, show "no search results" message.
            {
                TriggerCollectionUpdate(); // Updates the UI to show the message
                return;
            }

            if (_currentSortOption == SortingOption.Team && IsGrouped) //Group players by team if sorting by team and currently grouped.
            {
                var grouped = filteredPlayers
                    .GroupBy(p => p.Team) // Groups players by team name
                    .OrderBy(g => g.Key) // Sort groups alphabetically by team name, key = value grouped by
                    .Select(g => new PlayerGroup(g.Key, new ObservableCollection<Player>(g.OrderBy(p => p.LastName)), $"Team: {g.Key}")); // Each group turned into a PlayerGroup with a header.

                foreach (var group in grouped) // Adds each group to the PlayerGroups collection, which is bound to the UI.
                    PlayerGroups.Add(group);
            }
            else // If not grouped by team, grouped by LastName, Number etc, sort the filtered players and add them to a single group.
            {
                var sortedPlayers = SortPlayers(filteredPlayers, _currentSortOption, out string header); // out = method will return an extra value through a variable (in this case, header). List of stored players stored in sortedPlayers, string with header text stored in header).
                PlayerGroups.Add(new PlayerGroup("Filtered Players", new ObservableCollection<Player>(sortedPlayers), header)); // Adds the sorted players to a single group with a header.
            }

            TriggerCollectionUpdate(); // Updates the UI to show the filtered players.
        }

        // ============
        // Sorting
        // ============
        private void ApplySorting(SortingOption option)  // In what order should I see it? - sorts the player list based on the selected sorting option.
        {
            _currentSortOption = option;
            UpdateSortButtonColors(option);

            switch (option)
            {
                case SortingOption.Team: // Case 1: Sort by team
                    IsGrouped = true;
                    GroupPlayersByTeam();
                    break;
                case SortingOption.LastName: // Case 2: Sort by LastName/FirstName/Number
                case SortingOption.FirstName:
                case SortingOption.Number:
                    IsGrouped = false;
                    var sortedPlayers = SortPlayers(_allPlayers, option, out string header);
                    PlayerGroups.Clear(); // Clear old data
                    PlayerGroups.Add(new PlayerGroup("All Players", new ObservableCollection<Player>(sortedPlayers), header));
                    break;
            }

            if (!string.IsNullOrWhiteSpace(SearchText)) // If search text is present, apply the filter to the sorted players.
                ApplyFilter();
        }

        private void GroupPlayersByTeam() // Uses all players to group them by team and add them to the PlayerGroups collection.
        {
            var groupedPlayers = _allPlayers
                .GroupBy(p => p.Team)
                .OrderBy(g => g.Key)
                .Select(g => new PlayerGroup(g.Key, new ObservableCollection<Player>(g.OrderBy(p => p.LastName)), $"Team: {g.Key}"));

            PlayerGroups.Clear();
            foreach (var group in groupedPlayers)
                PlayerGroups.Add(group);
        }

        private List<Player> SortPlayers(List<Player> players, SortingOption option, out string header) // Header is set inside the method and returned to the caller (because of the out keyword).
        {
            switch (option) // Switch statement to determine how to sort the players.
            {
                case SortingOption.LastName:
                    header = "Sorted by Last Name";
                    return players.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
                case SortingOption.FirstName:
                    header = "Sorted by First Name";
                    return players.OrderBy(p => p.FirstName).ThenBy(p => p.LastName).ToList();
                case SortingOption.Number:
                    header = "Sorted by Player Number";
                    return players.OrderBy(p => ExtractNumericPart(p.PlayerNo)).ToList();
                default:
                    header = "Filtered Players";
                    return players;
            }
        }

        private void UpdateSortButtonColors(SortingOption option)
        {
            // Reset all to grey.
            TeamSortButtonColor = LastNameSortButtonColor = FirstNameSortButtonColor = NumberSortButtonColor = Color.FromArgb("#cccccc");

            // Highlight active sort.
            switch (option)
            {
                case SortingOption.Team: TeamSortButtonColor = Colors.Blue; break;
                case SortingOption.LastName: LastNameSortButtonColor = Colors.Blue; break;
                case SortingOption.FirstName: FirstNameSortButtonColor = Colors.Blue; break;
                case SortingOption.Number: NumberSortButtonColor = Colors.Blue; break;
            }
        }

        private int ExtractNumericPart(string playerNo) 
        {
            string digits = new string(playerNo.Where(char.IsDigit).ToArray()); // Goes through each character in the string and only keeps the digits.
            return int.TryParse(digits, out int number) ? number : 0; // Tries to convert digits to an integer, if it fails, returns 0.
        }

        private async Task DisplayAlert(string title, string message) // Displays a popup message with a title and message. Built into MAUI.
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        private void TriggerCollectionUpdate() // Forces UI to refresh two properties.
        {
            OnPropertyChanged(nameof(NoPlayersAvailable)); // Tells UI that property has changed
            OnPropertyChanged(nameof(ShowNoSearchResults));
        }

        // ============
        // INotifyPropertyChanged
        // ============
        public event PropertyChangedEventHandler PropertyChanged; // Event that is triggered when a property changes. 
        protected void OnPropertyChanged([CallerMemberName] string name = null) => // CallerMemberName = automatically fills in the name of the property that called the method. null allows for passing of name manually.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); // Invokes the PropertyChanged event, passing this object (the ViewModel) and the name of the property that changed.
    }

    // ============
    // PlayerGroup Class
    // ============
    public class PlayerGroup : ObservableCollection<Player> // // Represents a group of players (usually by team) for display in a grouped CollectionView; includes header text for the group.
    {
        public string TeamName { get; }
        public string GroupHeaderText { get; }

        public PlayerGroup(string teamName, ObservableCollection<Player> players, string headerText = null)
            : base(players)
        {
            TeamName = teamName;
            GroupHeaderText = headerText ?? $"Team: {teamName}";
        }
    }
}