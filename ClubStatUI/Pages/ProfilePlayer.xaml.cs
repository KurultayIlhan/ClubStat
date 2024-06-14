using ClubStat.Infrastructure;

using ClubStat.Infrastructure.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;


namespace ClubStatUI.Pages;

public partial class ProfilePlayer : ContentPage
{
	public ProfilePlayer(ProfilePlayerViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
        Loaded += ProfilePlayer_Loaded;
	}

    private async void ProfilePlayer_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is ILoadAsync loader)
        {
            await loader.ExecuteAsync().ConfigureAwait(true);
        }
        Loaded -= ProfilePlayer_Loaded;
    }
}
