
using DynamicIsland.Application.Media;
using DynamicIsland.Application.Ports;

namespace DynamicIsland.App.Shell;

public class TrayIconHost : IDisposable
{
    private readonly NotifyIcon _trayIcon;
    private readonly IslandWindow _window;
    private readonly MediaOrchestrator _orchestrator;
    private readonly IMediaTransport _transport;

    public TrayIconHost(IslandWindow window, MediaOrchestrator orchestrator, IMediaTransport transport)
    {
        _window = window;
        _orchestrator = orchestrator;
        _transport = transport;
        _trayIcon = new NotifyIcon
        {
            Text = "Dynamic Island",
            Visible = true, // İkon hemen belirir.
            Icon = SystemIcons.Application, // Geçici bir icon - TODO kendi .ico
        };
        ContextMenuStrip menu = new ContextMenuStrip();
        menu.Items.Add("Göster/Gizle", null, (_, _) => Toggle()); //EventHandler iki discard
        menu.Items.Add("Çıkış", null, (_, _) => System.Windows.Application.Current.Shutdown());
        menu.Items.Add("Test: Oynat/Duraklat", null, async (_, _) =>
        {
            var id = _orchestrator.ActiveSessionId;
            if (id != null) await _transport.TogglePlayPauseAsync(id);
        }); //TODO Seans 4 de silinecek.
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
