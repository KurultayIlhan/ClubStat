namespace ClubStatUI.Pages;

public partial class TeamPlayer : ContentPage
{
	public TeamPlayer(TeamPlayerViewModel viewModel)
	{
		InitializeComponent();
         BindingContext = viewModel;
	}
}