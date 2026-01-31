using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;

public partial class TeamsPage : ContentPage // Partial because the rest of the class is in the TeamsPage.xaml file
{
    public TeamsPage(TeamsViewModel viewModel) // Connects the TeamsPage(UI) to the TeamsViewModel
    {
        InitializeComponent(); // Loads XAML layout and connects it to this file
        BindingContext = viewModel; // Connects the TeamsPage(UI) to the TeamsViewModel so that Bindings work
    }
}