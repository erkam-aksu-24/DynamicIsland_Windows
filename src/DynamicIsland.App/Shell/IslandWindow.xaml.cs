using System.Windows;                            // Window, SystemParameters
using System.Windows.Interop;                    // WindowInteropHelper
using DynamicIsland.Platform.Hwnd; // NativeMethods (sabitler + P/Invoke)
using DynamicIsland.Presentation.Island;

namespace DynamicIsland.App.Shell;

public partial class IslandWindow : Window
{
    public IslandWindow(IslandViewModel  viewModel)
    {
        InitializeComponent();                   // XAML ağacını kurar — mevcut, dokunma
        DataContext = viewModel;
    }

    protected override void OnSourceInitialized(EventArgs e)   // ← BURAYA
    {
        base.OnSourceInitialized(e);               // her override'da base'i çağır — alışkanlık

        // 1) Konumlandırma
        var wa = SystemParameters.WorkArea;
        Left = wa.Left + (wa.Width - Width) / 2;
        Top  = wa.Top + 12;

        // 2) HWND al ve Win32 stillerini uygula
        var hwnd = new WindowInteropHelper(this).Handle;
        var ex = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE,
            ex | NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE);
    }
}
