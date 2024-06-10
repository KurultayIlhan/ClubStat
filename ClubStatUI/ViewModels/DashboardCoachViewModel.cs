using ClubStat.Infrastructure.Models;

namespace ClubStatUI.ViewModels
{
    [QueryProperty(nameof(User), nameof(User))]
    public partial class DashboardCoachViewModel : ObservableObject
    {
        [ObservableProperty]
        ClubStat.Infrastructure.Models.LoggedInUser? _user;

    }
}
