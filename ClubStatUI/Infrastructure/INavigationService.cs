// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Wed 05-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="INavigationService.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStatUI.Infrastructure
{
    public interface INavigationService
    {
        Task NavigateToAsync<TPage>() where TPage : Page;
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigation _navigation;

        public NavigationService(IServiceProvider serviceProvider, INavigation navigation)
        {
            _serviceProvider = serviceProvider;
            _navigation = navigation;
        }

        public async Task NavigateToAsync<TPage>() where TPage : Page
        {
            var page = _serviceProvider.GetRequiredService<TPage>();
            await _navigation.PushAsync(page);
        }
    }

}
