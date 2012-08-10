using System;

namespace slHackathonGame.Pages
{
    public partial class LaunchPage
    {
        // Constructor
        public LaunchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler to navigate to game page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayGame(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/GamePage.xaml", UriKind.Relative));
        }
    }
}