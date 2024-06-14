// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 14-May-2024
// ***********************************************************************
// <copyright file="DashboardPlayer.xaml.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Models;

using System.ComponentModel;

namespace ClubStatUI.Pages;

public partial class DashboardPlayer : ContentPage, INotifyPropertyChanged
{
    
    public DashboardPlayer(DashboardPlayerViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        Loaded += OnDashboardPlayer_Loaded;
    }

    private async void OnDashboardPlayer_Loaded(object? sender, EventArgs e)
    {
        if(this.BindingContext is ILoadAsync loader) 
        {
            await loader.ExecuteAsync()
                        .ConfigureAwait(true);
        }
        Loaded -= OnDashboardPlayer_Loaded;
    }


    private async void btnProfilePage(object sender, EventArgs e)
    {
        // Navigate to the profile page here
        // await Navigation.PushAsync(new ProfilePlayer());
        await Shell.Current.GoToAsync("//ProfilePlayer");
    }

    private async void btnStatsPage(object sender, EventArgs e)
    {
        // Navigate to TargetPage
        // await Navigation.PushAsync(new StatsPlayer());
        await Shell.Current.GoToAsync("//StatsPlayer");
    }
    private async void btnTeamPage(object sender, EventArgs e)
    {
        // Navigate to TargetPage
        //await Navigation.PushAsync(new TeamPlayer());
        await Shell.Current.GoToAsync("//TeamPlayer");
    }

    private void btnProfilePage(object sender, TappedEventArgs e)
    {

    }
    private async void btnAgendaPagina(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AgendaPlayer");
    }
    //public DashboardPlayer(DashboardPlayerViewModel model)
    //{
    //    InitializeComponent();
    //    // BindingContext = model;
    //}
}