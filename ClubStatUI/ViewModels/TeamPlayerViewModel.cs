// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Fri 24-May-2024
// ***********************************************************************
// <copyright file="TeamPlayerViewModel.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.ViewModels
{
    public partial class TeamPlayerViewModel : ObservableObject, ILoadAsync
    {
        private readonly IClubFactory _clubFactory;
        [ObservableProperty]
        private Player? _player;

        [ObservableProperty]
        List<Player> _team;
        public TeamPlayerViewModel(ILoginFactory loginFactory, IClubFactory clubFactory)
        {
            if (loginFactory.CurrentUser is Player player)
            {
                _player = player;
            }
            _team = new();
            //if (_player is not null)
            //{
            //    _team.Add(_player);
            //}
            _clubFactory = clubFactory;
        }


        public async Task ExecuteAsync()
        {
            try
            {
                if (Player is not null)
                {
                    var teamMates = await _clubFactory.GetTeamPlayers(Player.ClubId, Player.PlayersLeague, Player.PlayersLeagueLevel).ConfigureAwait(true);
                    Team = new(teamMates);
                }

            }
            catch (Exception ex)
            {
                Walter.Inverse.GetLogger("Team")?.LogException(ex);
            }
        }
    }
}
