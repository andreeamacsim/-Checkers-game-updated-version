using System.Windows.Input;
using System.IO;
using Checkers.Command;
using System.ComponentModel;
using System.Windows;

namespace Checkers.ViewModels
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        public ICommand SaveCommand { get; }

        private string _tempUsername;
        public string TempUsername
        {
            get { return _tempUsername; }
            set { _tempUsername = value; OnPropertyChanged("TempUsername"); }
        }

        private string _tempPassword;
        public string TempPassword
        {
            get { return _tempPassword; }
            set { _tempPassword = value; OnPropertyChanged("TempPassword"); }
        }

        public SignUpViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object parameter)
        {
           
            if (!string.IsNullOrEmpty(TempUsername) && !string.IsNullOrEmpty(TempPassword))
            {
                string filePath = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\Accounts.txt";
                string userAndPassword = $"{TempUsername};{TempPassword}";

                
                if (!File.Exists(filePath))
                {
                    
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine(userAndPassword);
                    }
                }
                else
                {
                   
                    File.AppendAllText(filePath, userAndPassword + "\n");
                }
            }
            else
            {
                MessageBox.Show("Please fill in both username and password.");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
