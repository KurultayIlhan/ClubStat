using MetroLog;

namespace ClubStatUI
{
    public partial class App : Application
    {
        public App(ILogger? logger = null)
        {
            try { 
            logger?.Debug("App start initialising components");
            InitializeComponent();
            logger?.Debug("Loading Appshell");
            MainPage = new AppShell();
            }catch(Exception ex)
            {
                logger?.Error(ex.ToString());
            }  
        }
    }
}
