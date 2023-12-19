using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace BCSH2SemestralniPraceCermakPetr.Views
{
    public partial class MainWindow : Window
    {
        private const double TopMargin = 55; // Height of the title bar for dragging maximizing etc.
        public MainWindow()
        {
            InitializeComponent();
        }
        public void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint((Visual)sender).Properties.IsLeftButtonPressed)
            {
                if (((Visual)sender).GetVisualRoot() is Window window && e.GetCurrentPoint(window).Position.Y <= TopMargin)
                {
                    if (e.ClickCount == 2)
                    {
                        if (window.WindowState == WindowState.Normal)
                        {
                            window.WindowState = WindowState.Maximized;
                        }
                        else
                        {
                            window.WindowState = WindowState.Normal;
                        }
                    }
                    else
                    {
                        window.BeginMoveDrag(e);
                    }
                }
            }
        }
    }
}