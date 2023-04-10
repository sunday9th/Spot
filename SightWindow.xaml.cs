using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Spot;

public partial class SightWindow
{
    private Point _mousePos;
    
    public SightWindow()
    {
        InitializeComponent();
    }
    
    private void SightWindow_OnClosed(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }
    
    private void Container_OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var position = e.GetPosition(SightImage);
        var m = SightMatrix.Value;
        if (e.Delta > 0)
        {
            m.ScaleAtPrepend(1.1, 1.1, position.X, position.Y);
        }
        else
        {
            m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, position.X, position.Y);
        }
        SightMatrix = new MatrixTransform(m);
        SightImage.RenderTransform = SightMatrix;
    }

    private void Container_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount <= 1) return;
        var m = new Matrix();
        SightMatrix = new MatrixTransform(m);
        SightImage.RenderTransform = SightMatrix;
    }

    private void Container_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(Container);
        _mousePos = position;
        SightImage.CaptureMouse();
    }

    private void Container_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        SightImage.ReleaseMouseCapture();
    }

    private void Container_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!SightImage.IsMouseCaptured) return;
        var pos = e.GetPosition(Container);
        var m = SightMatrix.Value;

        m.OffsetX += pos.X - _mousePos.X;
        m.OffsetY += pos.Y - _mousePos.Y;
        SightMatrix = new MatrixTransform(m);
        SightImage.RenderTransform = SightMatrix;
        _mousePos = pos;
    }
}