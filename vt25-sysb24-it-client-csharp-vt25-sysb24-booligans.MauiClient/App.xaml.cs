namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;
using System;


public partial class App : Application
{
    public App()
    {
        InitializeComponent(); // Loads XAML layout and connects it to this file
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(MainPage = new AppShell()); // Creates new Window instance and sets the root page to be an instance of AppShell
    }
}