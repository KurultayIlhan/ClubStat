using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.ViewModels
{
    [QueryProperty(nameof(User), nameof(User))]
    public partial class DashboardCoachViewModel : ObservableObject, ILoadAsync
    {
        private readonly IMatchFactory _matchFactory;
        [ObservableProperty]
        LoggedInUser? _user;

        [ObservableProperty]
        Match? _nextMatch;


        [ObservableProperty]
        Match? _previousMatch;
        public DashboardCoachViewModel(ILoginFactory loginFactory,IMatchFactory _matchFactory)
        {
            _user = loginFactory.CurrentUser;
            this._matchFactory = _matchFactory;
        }

        public async Task ExecuteAsync()
        {
            if (User is not null) 
            {
                NextMatch= await _matchFactory.GetCoachsNextMatch(User.UserId).ConfigureAwait(true);  
                PreviousMatch = await _matchFactory.GetPreviouseMatchForCoach(User.UserId).ConfigureAwait(true);  
            }
        }
    }
}
