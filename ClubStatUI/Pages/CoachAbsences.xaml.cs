using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public CoachAbsences()
        {
            InitializeComponent();

            // Set the binding context of the page to itself
            BindingContext = this;

            // Set the current date
            CurrentDate = DateTime.Today.ToString("D");
        }
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