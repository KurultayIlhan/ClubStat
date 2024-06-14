// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="FormationCoach.xaml.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.Pages;

public partial class FormationCoach : ContentPage
{
    
    private List<Frame> _assistIconFrames = new List<Frame>();


    public string DefaultFormation { get; set; } = "4-2-3-1";
    public FormationCoach(FormationCoachViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        UpdateFormationGrid("4-2-3-1"); // Set default formation
        Loaded += FormationCoach_Loaded;

    }

    private async void FormationCoach_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is ILoadAsync loader)
        {
            await loader.ExecuteAsync().ConfigureAwait(true);
        }
    }

    
    private void FormationPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedFormation = FormationPicker.SelectedItem as string;
        if (selectedFormation != null)
        {
            UpdateFormationGrid(selectedFormation);
        }
    }


    private void UpdateFormationGrid(string formation)
    {
        switch (formation)
        {
            case "4-2-3-1":

                Grid.SetRow(SP, 0);
                Grid.SetColumn(SP, 2);


                Grid.SetRow(LM, 1);
                Grid.SetColumn(LM, 0);


                Grid.SetRow(RM, 1);
                Grid.SetColumn(RM, 4);


                Grid.SetRow(CAM, 2);
                Grid.SetColumn(CAM, 2);


                Grid.SetRow(CML, 3);
                Grid.SetColumn(CML, 1);


                Grid.SetRow(CMR, 3);
                Grid.SetColumn(CMR, 3);

                Grid.SetRow(LB, 4);
                Grid.SetColumn(LB, 0);

                Grid.SetRow(RB, 4);
                Grid.SetColumn(RB, 4);

                Grid.SetRow(CBL, 5);
                Grid.SetColumn(CBL, 1);

                Grid.SetRow(CBR, 5);
                Grid.SetColumn(CBR, 3);


                Grid.SetRow(GK, 6);
                Grid.SetColumn(GK, 2);
                break;

            case "4-3-3":
                Grid.SetRow(SP, 0);
                Grid.SetColumn(SP, 2);

                Grid.SetRow(LM, 1);
                Grid.SetColumn(LM, 0);

                Grid.SetRow(RM, 1);
                Grid.SetColumn(RM, 4);

                Grid.SetRow(CAM, 3);
                Grid.SetColumn(CAM, 2);

                Grid.SetRow(CML, 3);
                Grid.SetColumn(CML, 1);

                Grid.SetRow(CMR, 3);
                Grid.SetColumn(CMR, 3);

                Grid.SetRow(LB, 4);
                Grid.SetColumn(LB, 0);

                Grid.SetRow(RB, 4);
                Grid.SetColumn(RB, 4);

                Grid.SetRow(CBL, 5);
                Grid.SetColumn(CBL, 1);

                Grid.SetRow(CBR, 5);
                Grid.SetColumn(CBR, 3);

                Grid.SetRow(GK, 6);
                Grid.SetColumn(GK, 2);
                break;

            case "4-4-2":

                Grid.SetRow(SP, 0);
                Grid.SetColumn(SP, 1);


                Grid.SetRow(LM, 3);
                Grid.SetColumn(LM, 0);


                Grid.SetRow(RM, 3);
                Grid.SetColumn(RM, 4);


                Grid.SetRow(CAM, 0);
                Grid.SetColumn(CAM, 3);


                Grid.SetRow(CML, 3);
                Grid.SetColumn(CML, 1);


                Grid.SetRow(CMR, 3);
                Grid.SetColumn(CMR, 3);


                Grid.SetRow(LB, 5);
                Grid.SetColumn(LB, 0);


                Grid.SetRow(RB, 5);
                Grid.SetColumn(RB, 4);


                Grid.SetRow(CBL, 5);
                Grid.SetColumn(CBL, 1);


                Grid.SetRow(CBR, 5);
                Grid.SetColumn(CBR, 3);


                Grid.SetRow(GK, 6);
                Grid.SetColumn(GK, 2);
                break;


        }
    }

    private async void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        if (sender is DragGestureRecognizer recognizer && recognizer.Parent.BindingContext is Player player)
        {
            e.Data.Properties["Player"] = player;
            e.Data.Text = player.FullName;
            if (BindingContext is FormationCoachViewModel viewModel)
            {
                await viewModel.OnDragStarting(e).ConfigureAwait(true);
            }
        }
    }
    private async void OnDrop(object sender, DropEventArgs e)
    {
        if (sender is DropGestureRecognizer recognizer)
        {
            var name = recognizer.Parent.AutomationId;

            if (BindingContext is FormationCoachViewModel viewModel && e.Data.Properties["Player"] is Player newPlayer)
            {
               await viewModel.MovePlayerOffField(name, newPlayer).ConfigureAwait(true);
            }            
        }
    }

}

