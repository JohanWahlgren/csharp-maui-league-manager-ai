using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;

public partial class TeamsReportPage : ContentPage
{
    public TeamsReportPage(TeamsReportViewModel viewModel)
    {
        // Constructor: Receives the TeamsReportViewModel via Dependency Injection
        InitializeComponent();
        BindingContext = viewModel;
    }
}