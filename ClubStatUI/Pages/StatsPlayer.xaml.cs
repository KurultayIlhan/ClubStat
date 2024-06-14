using ClubStat.Infrastructure;

namespace ClubStatUI.Pages;

public partial class StatsPlayer : ContentPage
{
	public StatsPlayer(StatsPlayerViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
        Loaded += StatsPlayer_Loaded;
	}

    private async void StatsPlayer_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is ILoadAsync loadStats)
        {
            await loadStats.ExecuteAsync().ConfigureAwait(true);
        }
        Loaded -= StatsPlayer_Loaded;
    }
}