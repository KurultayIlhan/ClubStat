// ***********************************************************************
// Assembly         : ClubstatUI
// Author           : Ilhan Kurultay
// Created          : Sun 09-Jun-2024
//
// Last Modified By : Walter Verhoeven
// Last Modified On : Sun 09-Jun-2024
// ***********************************************************************
// <copyright file="BooleanToVisibilityConverter.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Globalization;

namespace ClubStatUI.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }


        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
