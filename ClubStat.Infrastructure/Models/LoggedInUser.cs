// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-09-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-20-2024
// ***********************************************************************
// <copyright file="LoggedInUser.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClubStat.Infrastructure.Models;
/// <summary>
/// classe used by login service
/// </summary>
/// <param name="FullName">Full name of the person</param>
/// <param name="UserType">The role the person has in the app</param>
/// <param name="UserId">The Users ID in the system</param>
public partial class LoggedInUser:ObservableObject,IAuthenticatedUser
{
    [ObservableProperty]
    string _fullName  = string.Empty;

    [ObservableProperty]
    UserType _userType  = UserType.None;
    
    [ObservableProperty]
    Guid _userId  = Guid.Empty;
    public LoggedInUser(IAuthenticatedUser? user)
    {
        _fullName = user?.FullName??string.Empty;
        _userType = user?.UserType??UserType.None;
        _userId = user?.UserId??Guid.Empty;
    }
    public LoggedInUser()
    {
        
    }
}
