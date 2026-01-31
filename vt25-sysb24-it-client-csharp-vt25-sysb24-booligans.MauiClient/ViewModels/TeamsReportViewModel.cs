using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels
{
    // ==========
    // Helper Class: Wraps a Team with display-friendly text, used in CollectionView showing all the teams available for selection for generating a report
    // ==========
    public class TeamDisplayItem
    {
        public Team Team { get; set; }
        public string DisplayText => $"{Team?.TeamNo} - {Team?.TeamName}";
        public string TeamNo => Team?.TeamNo;
    }

    public class TeamsReportViewModel : INotifyPropertyChanged
    {
        // ==========
        // Fields (private backing data)
        // ==========
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private readonly IOpenAIService _openAIService;

        private string _teamNo;
        private string _reportText;
        private bool _isLoading;
        private bool _showSelectionReminder;
        private TeamDisplayItem _selectedTeam;
        private ObservableCollection<TeamDisplayItem> _teams;

        // ==========
        // Properties (publicly bound to UI)
        // ==========
        public ObservableCollection<TeamDisplayItem> Teams
        {
            get => _teams;
            set { _teams = value; OnPropertyChanged(); }
        }

        public TeamDisplayItem SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                _selectedTeam = value;

                if (_selectedTeam?.Team != null)
                {
                    TeamNo = _selectedTeam.TeamNo;
                    Console.WriteLine($"Selected team: {_selectedTeam.DisplayText}");
                }

                OnPropertyChanged();
            }
        }

        public string TeamNo
        {
            get => _teamNo;
            set { _teamNo = value; OnPropertyChanged(); }
        }

        public string ReportText
        {
            get => _reportText;
            set { _reportText = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public bool ShowSelectionReminder
        {
            get => _showSelectionReminder;
            set { _showSelectionReminder = value; OnPropertyChanged(); }
        }

        // ==========
        // Commands
        // ==========
        public ICommand GenerateReportCommand { get; }

        // ==========
        // Constructor
        // ==========
        public TeamsReportViewModel(ITeamService teamService, IPlayerService playerService, IOpenAIService openAIService)
        {
            _teamService = teamService;
            _playerService = playerService;
            _openAIService = openAIService;

            Teams = new ObservableCollection<TeamDisplayItem>();

            // Generate Report Command Logic
            GenerateReportCommand = new Command(async () =>
            {
                try
                {
                    if (SelectedTeam?.TeamNo == null)
                    {
                        ReportText = "Please select a team from the list above ⬆";

                        await Application.Current.MainPage.DisplayAlert(
                            "Team Selection Required",
                            "Please select a team from the list before generating a report.",
                            "OK");
                        return;
                    }

                    Console.WriteLine($"Generating report for team: {SelectedTeam.TeamNo}");
                    await GenerateReportAsync(SelectedTeam.TeamNo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GenerateReportCommand: {ex.Message}");
                    ReportText = $"Error: {ex.Message}";
                }
            });

            ReportText = "Select a team and click Generate Report to create a report.";

            // Load teams when the ViewModel is created
            Task.Run(async () => await LoadTeamsAsync());
        }

        // ==========
        // Load Teams from API
        // ==========
        private async Task LoadTeamsAsync()
        {
            try
            {
                IsLoading = true;
                var teams = await _teamService.GetTeamsAsync();
                Console.WriteLine($"Loaded {teams?.Count() ?? 0} teams from service");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Teams.Clear();

                    if (teams != null)
                    {
                        foreach (var team in teams)
                        {
                            if (team != null)
                            {
                                Teams.Add(new TeamDisplayItem { Team = team });
                                Console.WriteLine($"Added team: {team.TeamNo} - {team.TeamName}");
                            }
                        }
                    }

                    Console.WriteLine($"Teams collection now has {Teams.Count} items");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading teams: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ReportText = $"Error loading teams: {ex.Message}";
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ==========
        // Generate Report for Selected Team
        // ==========
        private async Task GenerateReportAsync(string teamNo)
        {
            if (string.IsNullOrWhiteSpace(teamNo))
            {
                ReportText = "Please select a valid team.";
                return;
            }

            IsLoading = true;
            ReportText = "Generating report...";

            try
            {
                var team = await _teamService.GetTeamAsync(teamNo);
                if (team == null)
                {
                    ReportText = $"Team with number {teamNo} not found.";
                    IsLoading = false;
                    return;
                }

                string teamInfo = $"Team: {team.TeamName} (ID: {team.TeamNo}), Wins: {team.Wins}, Losses: {team.Losses}";

                string playerInfo = "";
                if (team.Players != null && team.Players.Count > 0)
                {
                    foreach (var player in team.Players.Take(5))
                    {
                        playerInfo += $"{player.FirstName} {player.LastName} - Position: {player.Position}, Jersey Number: {player.PlayerShirtNo}\n";
                    }
                }
                else
                {
                    playerInfo = "No player information available.";
                }

                var report = await _openAIService.GenerateTeamReportAsync(teamInfo, playerInfo);
                ReportText = report;
            }
            catch (Exception ex)
            {
                ReportText = $"Error: {ex.Message}";
                Console.WriteLine($"Error generating report: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ==========
        // INotifyPropertyChanged Implementation
        // ==========
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}