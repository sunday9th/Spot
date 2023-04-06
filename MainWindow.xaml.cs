using System;
using System.Windows;
using Screen = System.Windows.Forms.Screen;

namespace Spot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnContentRendered(object? sender, EventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow == null)
            {
                return;
            }

            mainWindow.Title = "SpotPanel";

            var count = 0;
            count++;
            foreach (var screen in Screen.AllScreens)
            {
                var sightWindow = new SightWindow();
                sightWindow.Show();
                sightWindow.Top = screen.WorkingArea.Top;
                sightWindow.Left = screen.WorkingArea.Left;
                sightWindow.Height = screen.WorkingArea.Height;
                sightWindow.Width = screen.WorkingArea.Width;
                //split.Topmost = true;
                sightWindow.ShowInTaskbar = true;
                // sightWindow.Owner = mainWindow;
                sightWindow.Title = $"Spot #{count}"+" -> waiting image...";

                sightWindow.WindowState = WindowState.Maximized;
                count++;
            }
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}