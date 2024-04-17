using System.Windows.Input;
using Checkers.Command;
using Checkers.Models;
using Checkers.Views; 

namespace Checkers.ViewModels
{
    public class AccountViewModel : BaseNotification
    {
        public ICommand StartGameCommand { get; private set; }
        public ICommand ViewStatisticsCommand { get; private set; }

        public AccountViewModel()
        {
            StartGameCommand = new RelayCommand(OpenGameWindow);
            ViewStatisticsCommand = new RelayCommand(ViewStatistics);
        }

        private void OpenGameWindow(object parameter)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();

        }

        private void ViewStatistics(object parameter)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow();

            
            statisticsWindow.Show();
        }
    }
}
