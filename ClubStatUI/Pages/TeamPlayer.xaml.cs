using ClubStat.Infrastructure;

namespace ClubStatUI.Pages;

public partial class TeamPlayer : ContentPage
{
	public TeamPlayer(TeamPlayerViewModel viewModel)
	{
		InitializeComponent();
         BindingContext = viewModel;
        Loaded += TeamPlayer_Loaded;


	}

    private async void TeamPlayer_Loaded(object? sender, EventArgs e)
    {
        Loaded -= TeamPlayer_Loaded;
        try
        {
            if (BindingContext is ILoadAsync loader)
            {
                await loader.ExecuteAsync().ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            Walter.Inverse.GetLogger("TeamPlayer")?.LogException(ex);
        }
    }
}