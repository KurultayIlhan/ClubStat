namespace ClubStatUI.Pages;

public partial class DashboardCoach : ContentPage
{
	public DashboardCoach(DashboardCoachViewModel model)
	{
		InitializeComponent();
        BindingContext = model;
	}
    private async void btnFormation(object sender, EventArgs e)
    {
        // Navigate to TargetPage
        await Navigation.PushAsync(new FormationCoach());
    }
    private async void btnAbsences(object sender, EventArgs e)
    {
        // Navigate to TargetPage
        await Navigation.PushAsync(new CoachAbsences());
    }
}