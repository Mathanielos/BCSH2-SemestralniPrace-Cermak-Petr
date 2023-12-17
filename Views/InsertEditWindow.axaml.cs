using Avalonia.Controls;
using BCSH2SemestralniPraceCermakPetr.ViewModels;
using System;

namespace BCSH2SemestralniPraceCermakPetr.Views
{
    public partial class InsertEditWindow : Window
    {
        public InsertEditWindow()
        {
            InitializeComponent();
            Opened += EditWindow_Opened;
        }

        private void EditWindow_Opened(object sender, EventArgs e)
        {
            if (DataContext is InsertEditWindowViewModel viewModel)
            {
                viewModel.InsertOrEditCompleted += InsertOrEditCompleted;
            }
        }

        private void InsertOrEditCompleted(object sender, EventArgs e)
        {
            // Close the window
            Close();
        }
    }
}
