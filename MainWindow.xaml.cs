using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Screen = System.Windows.Forms.Screen;

namespace Spot;

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
        var builder = new ConfigurationBuilder()
            .AddJsonFile(path: "appsettings.json", optional: true);
        var configuration = builder.Build();

        var monitorSequences = configuration.GetSection("MonitorSequences")
            .GetChildren().Select(x=>int.Parse(x.Value!)-1).ToList();

        var mainWindow = Application.Current.MainWindow;

        if (mainWindow == null)
        {
            return;
        }

        mainWindow.Title = "SpotPanel";

        var monitorNum = Screen.AllScreens.Length;
        var count = 0;
        count++;
        foreach (var monitorSequence in monitorSequences.Take(monitorNum))
        {
            if (monitorSequence > monitorNum - 1)
            {
                MessageBox.Show(
                    mainWindow, 
                    "不正确的显示器顺序配置，请排除后启动应用",
                    "错误",MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            var screen = Screen.AllScreens[monitorSequence];
    
            var sightWindow = new SightWindow();
            sightWindow.Show();
            sightWindow.Top = screen.WorkingArea.Top;
            sightWindow.Left = screen.WorkingArea.Left;
            sightWindow.Height = screen.WorkingArea.Height;
            sightWindow.Width = screen.WorkingArea.Width;
            //split.Topmost = true;
            sightWindow.ShowInTaskbar = true;
            // sightWindow.Owner = mainWindow;
            sightWindow.Title = $"Spot #{count}" + " -> waiting image...";

            sightWindow.WindowState = WindowState.Maximized;
            count++;
        }
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }
}