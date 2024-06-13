using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClubStat.Infrastructure.Models;

/// <summary>
/// Team in the club
/// </summary>
public partial class Team: ObservableObject
{
    [ObservableProperty]
    int _age;

    [ObservableProperty]
    Club? _club;

    [ObservableProperty]
    string _teamAbrv = string.Empty;
 
    [ObservableProperty]
    ObservableCollection<Player> _players = new ObservableCollection<Player>();


    public string TeamName
    {
        get
        {
            if (Age < 21)
            {
                return $"U{Age + 1} {TeamAbrv}";
            }
            return $"First Team{TeamAbrv}";
        }
    }
}
