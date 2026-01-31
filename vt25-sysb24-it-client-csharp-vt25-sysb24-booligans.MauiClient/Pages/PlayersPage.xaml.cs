using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;

public partial class PlayersPage : ContentPage // Partial because the rest of the class is in the PlayersPage.xaml file
{
	public PlayersPage(PlayersViewModel viewModel) // Connects the PlayersPage(UI) to the PlayersViewModel
	{
		InitializeComponent(); // Loads XAML layout and connects it to this file
		BindingContext = viewModel; // Connects the PlayersPage(UI) to the PlayersViewModel so that Bindings work
	}
}