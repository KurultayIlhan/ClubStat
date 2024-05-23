using CommunityToolkit.Mvvm.ComponentModel;

namespace ClubStat.Infrastructure.Models;

/// <summary>
/// OnScreenDashboardPlayer these are the personalstatistics of the user using the application
/// </summary>
public partial class PersonalStatisticsView : ObservableObject
{
    [ObservableProperty]
    int _passesMade;

    [ObservableProperty]
    int _passesScored;

    [ObservableProperty]
    int _shotsMade;

    [ObservableProperty]
    int _shotsScored;

}