using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;
namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("teams", typeof(TeamsPage));
        Routing.RegisterRoute("players", typeof(PlayersPage));
        Routing.RegisterRoute("teamsreport", typeof(TeamsReportPage)); // Add this line
    }

    // Handle navigation after the shell is loaded
    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        if (args.Previous == null) // Checks if this is the first navigation when app starts
        {
            await Task.Delay(100); // Small delay to ensure shell is ready
            CurrentItem = Items.FirstOrDefault(); // Navigate to first tab/item
        }
    }
}
