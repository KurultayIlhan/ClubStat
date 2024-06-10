namespace ClubStatUI.Pages;

public partial class FormationCoach : ContentPage
{
    private bool _singleTapWaiting = false;
    private List<Frame> _assistIconFrames = new List<Frame>();
    public string DefaultFormation { get; set; } = "4-2-3-1";
    public FormationCoach()
    {
        InitializeComponent();
        UpdateFormationGrid("4-2-3-1"); // Set default formation
    }

    private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {
        if (!_singleTapWaiting)
        {

            ballIconFrame.IsVisible = !ballIconFrame.IsVisible;


            _singleTapWaiting = true;


            await Task.Delay(200);

            _singleTapWaiting = false;
        }
    }

    [Obsolete]
    private void OnTapAssist(object sender, EventArgs e)
    {

        Frame newAssistIconFrame = new Frame
        {
            HeightRequest = 20,
            WidthRequest = 20,
            CornerRadius = 70,
            HorizontalOptions = LayoutOptions.Center,
            IsClippedToBounds = true,
            Padding = 0,
            TranslationX = 50,
            TranslationY = 20,
            BackgroundColor = Color.FromHex("#262626"),
            BorderColor = Color.FromHex("#D6D6D6"),
            Margin = new Thickness(0, 0, 0, 0),
            ZIndex = 14,
            IsVisible = true
        };


        Image assistIcon = new Image
        {
            HeightRequest = 10,
            WidthRequest = 10,
            Source = "assisticon.png",
            ZIndex = 11
        };


        newAssistIconFrame.Content = assistIcon;


        double offsetX = _assistIconFrames.Count * 10;
        int offsetZ = _assistIconFrames.Count + 1;


        newAssistIconFrame.TranslationX = offsetX;
        newAssistIconFrame.ZIndex = offsetZ;


        CAM.Children.Add(newAssistIconFrame);
    }



    private void FormationPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedFormation = FormationPicker.SelectedItem as string;
        if (selectedFormation != null)
        {
            UpdateFormationGrid(selectedFormation);
        }
    }

    private void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        var image = sender as Image;
        if (image != null)
        {
            var draggedImageSource = image.Source as FileImageSource;
            if (draggedImageSource != null)
            {

                e.Data.Properties.Add("ImageSource", draggedImageSource.File);
            }
        }
    }
    private void OnDrop(object sender, DropEventArgs e)
    {
        if (e.Data.Properties.ContainsKey("ImageSource"))
        {
            var draggedImageSource = e.Data.Properties["ImageSource"] as string;

            if (draggedImageSource != null)
            {
                var dropFrame = sender as Frame;
                var image = dropFrame.Content as Image;
                if (image != null)
                {
                    image.Source = draggedImageSource;
                }
            }
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
}

