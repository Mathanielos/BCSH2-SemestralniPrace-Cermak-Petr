using Avalonia.Controls;
using BCSH2SemestralniPraceCermakPetr.ViewModels;
using System;

namespace BCSH2SemestralniPraceCermakPetr.Views
{
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
            Opened += EditWindow_Opened;
        }

        private void EditWindow_Opened(object sender, EventArgs e)
        {
            if (DataContext is EditWindowViewModel viewModel)
            {
                viewModel.EditCompleted += EditCompleted;
            }
        }

        private void EditCompleted(object sender, EventArgs e)
        {
            // Close the window
            Close();
        }
    }
}
