using ClubStat.Infrastructure;

namespace ClubStatUI.Pages;

public partial class AgendaPlayer : ContentPage
{
    public AgendaPlayer(AgendaPlayerViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        Loaded += AgendaPlayer_Loaded;
    }

    private async void AgendaPlayer_Loaded(object? sender, EventArgs e)
    {
        Loaded -= AgendaPlayer_Loaded;
        if (BindingContext is ILoadAsync loader)
        {
            await loader.ExecuteAsync().ConfigureAwait(true);
        }
    }
}