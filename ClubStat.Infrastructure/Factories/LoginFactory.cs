// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="LoginFactory.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>
// TASK 7: Integrate Login with REST API
// </summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Factories
{
    public interface ILoginFactory
    {
        //If null unknown user if true player else coach.
        Task<IAuthenticatedUser?> Login(string username, string password);
        LoggedInUser? CurrentUser { get; }
    }
    internal sealed class LoginFactory : ApiBasedFactory, ILoginFactory
    {
        private readonly IProfilePictureFactory _pictureFactory;

        public LoginFactory(IConfiguration configuration, IHttpClientFactory clientFactory, IProfilePictureFactory pictureFactory)
            : base(configuration, clientFactory)
        {
            _pictureFactory = pictureFactory;
#if DEBUG
            //CurrentUser = new Player()
            //{
            //    ClubId = 4,
            //    DateOfBirth = new DateTime(2009, 12, 07),
            //    FullName = "Ronaldo",
            //    PlayerAttitude = 7,
            //    PlayerMotivation = 6,
            //    PlayersLeague = 15,
            //    PlayersLeagueLevel = 'A',
            //    UserId = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6"),
            //    UserType = UserType.Player
            //};
#endif

        }

        public LoggedInUser? CurrentUser { get; private set; }

        // fake login for offline development        
        public async Task<IAuthenticatedUser?> Login(string username, string password)
        {
            // Generates and entry in the ILogger null or empty
            Walter.Guard.EnsureNotNullOrEmpty(username);
            Walter.Guard.EnsureNotNullOrEmpty(password);
            if (string.Equals(username, CurrentUser?.FullName, StringComparison.Ordinal))
                return CurrentUser;

            var model = new LogInUser() { Username = username, Password = password };
            var answer = await base.PostAsync<LogInResult>(MagicStrings.LoginApi, model).ConfigureAwait(false);
            //remove cashed user
            CurrentUser = null;

            if (answer is null) { return null; }


            switch (answer.UserType)
            {
                case UserType.Player:

                    var player = await GetAsync<Player>(MagicStrings.PlayerUrl(answer.UserId)).ConfigureAwait(false);
                    if (player is not null)
                    {
                        var bytes = await _pictureFactory.GetProfilePictureForUserAsync(answer.UserId);
                        player.ProfileImageBytes = bytes;
                    }
                    CurrentUser = player;
                    break;

                case UserType.Coach:
                    CurrentUser = await GetAsync<Coach>(MagicStrings.CoachUrl(answer.UserId)).ConfigureAwait(false);
                    break;
                case UserType.Delegee:

                    break;
            }
            if(CurrentUser is not null) 
            {
                CurrentUser.UserId = answer.UserId;
                CurrentUser.UserType = answer.UserType;
                
            }
            //return cashed user
            return CurrentUser;

        }
    }
}
