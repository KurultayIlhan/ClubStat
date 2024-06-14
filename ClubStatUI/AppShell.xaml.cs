// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : 02-09-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-20-2024
// ***********************************************************************
// <copyright file="AppShell.xaml.cs" company="ClubStatUI">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStatUI
{
    /// <summary>
    /// Application class amanged by Maui
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppShell"/> class.
        /// </summary>
        /// <remarks>Default constructor</remarks>
        public AppShell()
        {
            /*hamburger menu*/
            InitializeComponent();
            // Normaal gezien zijn de routes hier gedefinierd maar 
            // ik doe dat in de dependency injection zodat ik ze niet vergeet
            Builder.ClubStatsDependecyInjectionHelper.RegisterRoutes();
        }
    }
}
