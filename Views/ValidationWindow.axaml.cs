using Avalonia.Controls;
using BCSH2SemestralniPraceCermakPetr.ViewModels;
using System;

namespace BCSH2SemestralniPraceCermakPetr.Views
{
    public partial class ValidationWindow : Window
    {
        public ValidationWindow()
        {
            InitializeComponent();
            Opened += EditWindow_Opened;
        }
        private void EditWindow_Opened(object sender, EventArgs e)
        {
            if (DataContext is ValidationViewModel viewModel)
            {
                viewModel.ValidationCompleted += ValidationCompleted;
            }
        }

        private void ValidationCompleted(object sender, EventArgs e)
        {
            // Close the window
            Close();
        }
    }
}
