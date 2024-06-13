namespace ClubStat.Infrastructure.Models;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class UpcomingMatchView : ObservableObject
{
    [ObservableProperty]
    Division _division = Division.None;

    [ObservableProperty]
    bool _remindMe;

    [ObservableProperty]
    Match? _match;
    public UpcomingMatchView()
    {
        
    }


}
