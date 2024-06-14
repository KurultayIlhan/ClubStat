// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="CoachAbsences.xaml.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.Pages;

public partial class CoachAbsences : ContentPage
{
    public List<string> SampleNames { get; } = new List<string>
        {
            "John Doe",
            "Jane Smith",
            "Michael Johnson",
            "Emily Brown",
            "David Lee"
        };

    public string CurrentDate { get; }

    ClubStat.Infrastructure.Models.Match _match;
    public CoachAbsences(ClubStat.Infrastructure.Models.Match match)
    {
        InitializeComponent();
        _match = match;
        // Set the binding context of the page to itself
        BindingContext = this;

        // Set the current date
        CurrentDate = DateTime.Today.ToString("D");
    }

    public Match LastMatch => _match;

    private void OnSelectButtonClicked(object sender, EventArgs e)
    {
        var selectedNames = new List<string>();

        if (nameListView.SelectedItem != null)
        {
            selectedNames.Add((string)nameListView.SelectedItem);
        }

        DisplayAlert("Selected Names", string.Join(", ", selectedNames), "OK");
    }
}