// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan
// Created          : Mon 10-Jun-2024
//
// Last Modified By : Ilhan
// Last Modified On : Mon 10-Jun-2024
// ***********************************************************************
// <copyright file="BytesToImageConverter.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;

using ClubStatUI.Infrastructure;

using System.Globalization;

namespace ClubStatUI.Converter
{
    public class BytesToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                //repeated calls to the same imahe will cash by the extension method
                return bytes.ToImage();
            }

            return ClubMember._noProfileImage.ToImage();

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
