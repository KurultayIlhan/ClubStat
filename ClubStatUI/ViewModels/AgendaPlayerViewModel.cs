// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : Sat 01-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="AgendaPlayerViewModel.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.ViewModels
{
    public partial class AgendaPlayerViewModel : ObservableObject, ILoadAsync
    {
        private readonly IMatchFactory _matchFactory;

        [ObservableProperty]
        private Player? _player;
        [ObservableProperty]
        private List<Match> _matches;

        public AgendaPlayerViewModel(IMatchFactory matchFactory, ILoginFactory loginFactory)
        {
            _matches = new();
            _matchFactory = matchFactory;
            if (loginFactory.CurrentUser is Player player)
            {
                _player = player;
            }
        }




        public async Task ExecuteAsync()
        {
            if (Player is not null)
            {
                var agenda = await _matchFactory.GetPlayersAgendaAsync(Player.UserId).ConfigureAwait(true);
                if (agenda != null && agenda.Count > 0)
                {
                    Matches = new(agenda.OrderBy(o => o.MatchDate));
                }

            }
        }
    }
}
