using System.IO;
using System.Windows;
using System.Windows.Input;
using Checkers.Command;
using Checkers.Helpers;
using Checkers.Models;

namespace Checkers.ViewModels
{
    public class LoginViewModel : BaseNotification
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand SaveCommand { get; }

        public event EventHandler LoginSuccessful;

        public LoginViewModel()
        {
            SaveCommand = new RelayCommand(SaveCredentials, CanSaveCredentials);
        }

        private bool CanSaveCredentials(object parameter)
        {
            
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void SaveCredentials(object parameter)
        {
            string filePath = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\Accounts.txt";

            if (!File.Exists(filePath))
            {
                MessageBox.Show("The credentials file does not exist.");
                return;
            }

            var credentialsFound = CheckCredentials(filePath, Username, Password);

            if (credentialsFound)
            {
                SessionData.CurrentUserName = Username;
                OnLoginSuccessful(); 
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private bool CheckCredentials(string filePath, string username, string password)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2)
                {
                    if (parts[0].Equals(username, StringComparison.OrdinalIgnoreCase) &&
                        parts[1].Equals(password))
                    {
                        return true; 
                    }
                }
            }

            return false; 
        }

        protected virtual void OnLoginSuccessful()
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty); 
        }
    }
}
