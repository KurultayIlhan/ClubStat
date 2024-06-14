// ***********************************************************************
// Assembly         : ClubstatUI
// Author           : Ilhan Kurultay
// Created          : Thu 06-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 06-Jun-2024
// ***********************************************************************
// <copyright file="UtcToLocalDateTimeConverter.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary>
// Convert database UTC dates to local time
// </summary>
// ***********************************************************************
using System.Globalization;

namespace ClubStatUI.Converter
{
    public class UtcToLocalDateTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime utcDate)
            {
                return utcDate.ToLocalTime();
            }
            return value;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime localDate)
            {
                return localDate.ToUniversalTime();
            }
            return DateTime.Now;
        }
    }
}
