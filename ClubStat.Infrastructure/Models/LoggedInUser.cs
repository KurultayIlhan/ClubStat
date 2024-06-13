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

    string _fullName  = string.Empty;
    UserType _userType  = UserType.None;
    Guid _userId  = Guid.Empty;

    public string FullName { get => _fullName; set => SetProperty(ref _fullName , value); }
    public UserType UserType { get => _userType; set => SetProperty(ref _userType , value); }
    public Guid UserId { get => _userId; set => SetProperty(ref _userId, value); }

    public LoggedInUser(IAuthenticatedUser? user)
    {
        FullName = user?.FullName??string.Empty;
        UserType = user?.UserType??UserType.None;
        UserId = user?.UserId??Guid.Empty;
    }
    public LoggedInUser(string fullName, UserType userType, Guid userId)
    {
        _fullName = fullName;
        _userType = userType;
        _userId = userId;
    }
    public LoggedInUser()
    {
        
    }
}
