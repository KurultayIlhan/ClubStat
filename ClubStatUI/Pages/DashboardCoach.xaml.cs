// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="DashboardCoach.xaml.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;

namespace ClubStatUI.Pages;

public partial class DashboardCoach : ContentPage
{
	public DashboardCoach(DashboardCoachViewModel model)
	{
		InitializeComponent();
        BindingContext = model;
        Loaded += DashboardCoach_Loaded;
	}

    private async void DashboardCoach_Loaded(object? sender, EventArgs e)
    {
        Loaded -= DashboardCoach_Loaded;
        if (BindingContext is ILoadAsync loader)
        {
            await loader.ExecuteAsync().ConfigureAwait(true);
        }
    }

    private async void btnFormation(object sender, EventArgs e)
    {
        if(BindingContext is DashboardCoachViewModel dashboard && dashboard.NextMatch is not null)
        // Navigate to TargetPage
        await Shell.Current.GoToAsync(nameof(ClubStatUI.Pages.FormationCoach));
    }
    private async void btnAbsences(object sender, EventArgs e)
    {
        if(BindingContext is DashboardCoachViewModel dashboard && dashboard.PreviousMatch is not null)
        // Navigate to TargetPage
         await Shell.Current.GoToAsync(nameof(ClubStatUI.Pages.CoachAbsences));

    }
}