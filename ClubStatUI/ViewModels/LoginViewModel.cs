// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : 02-12-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-12-2024
// ***********************************************************************
// <copyright file="LoginViewModel.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Factories;
using CommunityToolkit.Mvvm.Input;

using System.Diagnostics;

namespace ClubStatUI.ViewModels
{
    /// <summary>
    /// Class LoginViewModel.
    /// Implements the <see cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject" />
    /// </summary>
    /// <seealso cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject" />
    public partial class LoginViewModel : ObservableObject
    {
        /// <summary>
        /// The login factory
        /// </summary>
        private readonly ILoginFactory _loginFactory;

        /// <summary>
        /// The login name
        /// </summary>
        [ObservableProperty]
        string? _loginName;
        /// <summary>
        /// The password
        /// </summary>
        [ObservableProperty]
        string? _password;

        //constructor
        //TODO: Implement login factory
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="loginFactory">The login factory.</param>
        public LoginViewModel(ILoginFactory loginFactory)
        {
            this._loginFactory = loginFactory;
        }
        /// <summary>
        /// Called when [login].
        /// </summary>
        [RelayCommand]
        async Task OnLogin()
        {
            if (string.IsNullOrEmpty(LoginName) || string.IsNullOrEmpty(Password))
            {
                await Shell.Current.DisplayAlert("Wrong username or password", "You didn't enter any", "ok");
                return;
            }

            //Als succes is null, dan is de user onbekend
            //show msg and exit
            if (_loginFactory.CurrentUser is null)
            {

                await _loginFactory.Login(LoginName, Password).ConfigureAwait(true);
            }


            // LoggedInUser is de nieuwe instantie waar de user aangemeld heeft,
            // en die geef je als parameter mee aan de navigatie voor de dashboards
            // Als het succes is word je aangemeld als een speler, anders als een coach
            // Login = new ClubStat.Infrastructure.Models.LoggedInUser(LoginName, succes);
            var succes = _loginFactory.CurrentUser?.UserType ?? ClubStat.Infrastructure.Models.UserType.None;
            if(succes == ClubStat.Infrastructure.Models.UserType.None)
            {
                Password = string.Empty;
               await Shell.Current.DisplayAlert("Wrong username or password", "User not found", "ok");
              return;
            }
            try
            {
                switch (succes)
                {
                    case ClubStat.Infrastructure.Models.UserType.Player:

                        await Shell.Current.GoToAsync(nameof(Pages.DashboardPlayer));
                        break;
                    case ClubStat.Infrastructure.Models.UserType.Coach:

                        await Shell.Current.GoToAsync(nameof(Pages.DashboardCoach));
                        break;
                    case ClubStat.Infrastructure.Models.UserType.Delegee:
                        throw new NotImplementedException("TODO: we need to create delegee screen");
                    default:
                        await Shell.Current.DisplayAlert("Wrong username or password", "User not found", "ok");
                        return;
                }
            }
            catch (Exception ex) 
            { 
                //async task can crash at strange locations, this should allow me to nind where
                Debug.WriteLine(ex.ToString());

                Walter.Inverse.GetLogger<LoginViewModel>()?.LogException(ex);
            }
        }

    }
}
