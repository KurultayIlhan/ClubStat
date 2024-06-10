using ClubStat.Infrastructure.Models;

namespace ClubStatUI.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ViewModels.LoginViewModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
