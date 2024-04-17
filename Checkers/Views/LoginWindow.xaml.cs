using Checkers.ViewModels;

using System.Windows;

namespace Checkers.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    // În LoginWindow.xaml.cs
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var viewModel = new LoginViewModel();
            DataContext = viewModel;

            
            viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
        }

        private void ViewModel_LoginSuccessful(object sender, EventArgs e)
        {
           
            AccountView accountView = new AccountView();
            accountView.Show();

            
            this.Close();
        }
    }

}
