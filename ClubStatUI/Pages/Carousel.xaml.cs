using ClubStatUI.ViewModels;
namespace ClubStatUI.Pages;

public partial class Carousel : ContentPage
{
	public Carousel()
    {
        InitializeComponent();
        var items = new List<CarouselViewModel>
		{
			new() {Title="WELCOME TO YOUR NEW FOOTBALL CLUB TRACKER APP", Description="Bringing you all the latest news and statistics.", Image = "", ImageBg="carouselone.png",},
			new() {Title="MATCHDAY", Description="Planning before and after matchdays are easier using a solid todo and alert schedule you can turn on.", Image="message.png", ImageBg="carouseltwo.png"},
			new() {Title="POINT PRECISE TRACKING STATS", Description="Bringing you all the latest news and statistics.",Image="carouselstat.png", ImageBg="carouselone.png"},
			new() { Description="Log in now to unlock a world of possibilities. Enjoy easy access to features designed just for you. Whether you're here for fun or productivity, your personalized experience is just a click away."},
        };
		carouselView.ItemsSource = items;
	}

}