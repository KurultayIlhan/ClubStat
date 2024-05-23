namespace ClubStat.Infrastructure.Models;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ClubIcon: ObservableObject
{
    [ObservableProperty]
    string _clubName;

    [ObservableProperty]
    string _logoResourceUrl;

    public ClubIcon(string clubName, string logoResourceUrl)
    {
        _clubName = clubName;
        _logoResourceUrl = logoResourceUrl;
    }
}