using System;
using System.Configuration;
using System.IO;
// using System.Configuration;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;

// using Microsoft.Extensions.Configuration;

namespace Spot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TcpServer _server;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var settingPort = configuration["TcpPort"];
            
            if (!int.TryParse(settingPort, out var port))
            {
                port = 8087;
            }

            _server = new TcpServer(IPAddress.Parse("0.0.0.0"), port);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            _server.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _server.Stop();
            base.OnExit(e);
        }

        private static void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var ex = e.Exception.InnerException;
            if (ex is null)
            {
                return;
            }

            MessageBox.Show(
                "意料外的错误：" + Environment.NewLine
                          + "错误源：" + ex.Source + Environment.NewLine
                          + "错误信息：" + e.Exception.Message + Environment.NewLine
                          + "详细信息：" + ex.Message + Environment.NewLine
                          + "报错区域：" + ex.StackTrace
            );
        }
    }
}