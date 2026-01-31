using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;

public partial class MainPage : ContentPage // Partial because the rest of the class is in the MainPage.xaml file
{
    public MainPage(MainViewModel viewModel) // Connects the MainPage(UI) to the MainViewModel
    {
        InitializeComponent(); // Loads XAML layout and connects it to this file
        BindingContext = viewModel; // Connects the MainPage(UI) to the MainViewModel so that Bindings work
    }
}