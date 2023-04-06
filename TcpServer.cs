using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Spot;

public class TcpServer
{
    private TcpListener _listener;
    private Thread _thread;
    private Dispatcher _dispatcher;
    private bool _isRunning;

    public TcpServer(IPAddress ipAddress, int port)
    {
        _listener = new TcpListener(ipAddress, port);
        _thread = new Thread(ListenForClients);
        _dispatcher = Application.Current.Dispatcher;
        _isRunning = false;
    }

    public void Start()
    {
        _listener.Start();
        _isRunning = true;

        _thread.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
    }

    private void ListenForClients()
    {
        var bytes = new byte[256];
        var ok = "ok"u8.ToArray();
        var fail = "fail"u8.ToArray();
        var invalidRequest = "invalid request"u8.ToArray();
        var notFound = "image not found"u8.ToArray();
        var outOfRange = "index out of range"u8.ToArray();

        while (_isRunning)
        {
            try
            {
                var client = _listener.AcceptTcpClient();

                var stream = client.GetStream();
                int i;

                MainWindow? mainWindow = default;
                List<SightWindow>? sightWindows = default;
                _dispatcher.Invoke(() =>
                {
                    mainWindow = Application.Current.Windows.Cast<Window>()
                        .FirstOrDefault(w => w is MainWindow) as MainWindow;
                    sightWindows = Application.Current.Windows.Cast<Window>()
                        .OrderBy(window => window.Title)
                        .Where(window => window is SightWindow)
                        .Cast<SightWindow>()
                        .ToList();
                });
                if (mainWindow is null || sightWindows is null)
                {
                    stream.Write(fail, 0, fail.Length);
                    client.Close();
                    continue;
                }

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var data = Encoding.UTF8.GetString(bytes, 0, i);

                    var datas = data.Split(",", 2, StringSplitOptions.TrimEntries);
                    if (datas.Length != 2 || !int.TryParse(datas[0], out var index) || datas[1] == string.Empty)
                    {
                        stream.Write(invalidRequest, 0, invalidRequest.Length);
                        break;
                    }

                    var imagePath = datas[1];
                    if (index > sightWindows.Count)
                    {
                        stream.Write(outOfRange, 0, outOfRange.Length);
                        break;
                    }

                    index--;

                    _dispatcher.Invoke(() =>
                    {
                        var sightWindow = sightWindows[index];
                        sightWindow.Title = $"Spot #{index + 1}";
                        sightWindow.Title += $" -> Image: \"{imagePath}\"";
                        sightWindow.SightImage.Source = default;

                        if (File.Exists(imagePath))
                        {
                            sightWindow.SightImage.Source = new BitmapImage(new Uri(imagePath));
                            sightWindow.Title += " -> OK";
                            stream.Write(ok, 0, ok.Length);
                        }
                        else
                        {
                            sightWindow.Title += " -> Error: image not found";
                            stream.Write(notFound, 0, notFound.Length);
                        }
                    });
                }

                client.Close();
            }
            catch (SocketException)
            {
            }
        }
    }
}