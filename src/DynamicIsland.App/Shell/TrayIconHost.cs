using System.Windows.Forms;
using System.Drawing;

namespace DynamicIsland.App.Shell;

public class TrayIconHost : IDisposable
{
    //Alanlar
    private NotifyIcon _trayIcon = new NotifyIcon();
    private IslandWindow _window = new IslandWindow();

    public TrayIconHost(IslandWindow window)
    {
        _window = window;
        _trayIcon = new NotifyIcon
        {
            Text = "Dynamic Island",
            Visible = true, // İkon hemen belirir.
            Icon = SystemIcons.Application, // Geçici bir icon - TODO kendi .ico
        };
        ContextMenuStrip menu = new ContextMenuStrip();
        menu.Items.Add("Göster/Gizle", null, (_, _) => Toggle()); //EventHandler iki discard
        menu.Items.Add("Çıkış", null, (_, _) => System.Windows.Application.Current.Shutdown());
        _trayIcon.ContextMenuStrip = menu;
    }

    private void Toggle()
    {
        if (_window.IsVisible)
        {
           _window.Hide();
        }
        else
        {
            _window.Show();
        }
    }


    public void Dispose()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
    }
}
