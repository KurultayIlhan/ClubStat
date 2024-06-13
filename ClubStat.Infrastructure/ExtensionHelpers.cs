// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="MagicStrings.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>
// strings used by the application
// </summary>
// ***********************************************************************
namespace ClubStat.Infrastructure
{
    public static class ExtensionHelpers
    {
        public static ClubStat.Infrastructure.Models.Division ToDivision(this char letter)
        { 
            return letter switch 
            {   
                'a' => Division.Div1,
                'A' => Division.Div1,
                'b' => Division.Div2,
                'B' => Division.Div2,
                'c' => Division.Div3,
                'C' => Division.Div3,
                _=> Division.None
            };
        }
    }
}
