﻿
using Checkers.Views;
using System.Windows;

namespace Checkers.Views
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.DataContext = this.DataContext;
            aboutWindow.Show();
          
        }

        
    }
}