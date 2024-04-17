using System.IO;
using Checkers.Models; 
using System.ComponentModel;
using Checkers.Helpers;

namespace Checkers.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private int _redWins;
        private int _whiteWins;
        private int _maxPiecesRemaining;

        public string CurrentUserName
        {
            get { return SessionData.CurrentUserName; }
        }
        public int RedWins
        {
            get { return _redWins; }
            set
            {
                _redWins = value;
                OnPropertyChanged(nameof(RedWins));
            }
        }

        public int WhiteWins
        {
            get { return _whiteWins; }
            set
            {
                _whiteWins = value;
                OnPropertyChanged(nameof(WhiteWins));
            }
        }

        public int MaxPiecesRemaining
        {
            get { return _maxPiecesRemaining; }
            set
            {
                _maxPiecesRemaining = value;
                OnPropertyChanged(nameof(MaxPiecesRemaining));
            }
        }

        public StatisticsViewModel()
        {
            LoadStatistics();
        }

        public void LoadStatistics()
        {
            string path = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\file.txt";
            string currentUser = SessionData.CurrentUserName;

            
            RedWins = 0;
            WhiteWins = 0;
            MaxPiecesRemaining = 0;

            if (!File.Exists(path))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                bool isCurrentUserSection = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("User: "))
                    {
                        isCurrentUserSection = line.Contains(currentUser);
                        continue;
                    }

                    if (isCurrentUserSection)
                    {
                        if (line.Contains("Red player wins"))
                        {
                            RedWins += int.Parse(line.Split(':')[1].Trim());
                        }
                        else if (line.Contains("White player wins"))
                        {
                            WhiteWins += int.Parse(line.Split(':')[1].Trim());
                        }
                        else if (line.Contains("Max pieces remaining for a winner"))
                        {
                            MaxPiecesRemaining = int.Parse(line.Split(':')[1].Trim());
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
