namespace ClubStat.Infrastructure.Models;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class UpcomingMatchView : ObservableObject
{
    [ObservableProperty]
    Division _division = Division.None;

    [ObservableProperty]
    bool _remindMe;

    [ObservableProperty]
    DateTime _matchDate;

    [ObservableProperty]
    ClubIcon? _homeTeam;

    [ObservableProperty]
    ClubIcon? _awayTeam;
}
