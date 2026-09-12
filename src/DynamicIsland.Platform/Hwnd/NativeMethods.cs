using System.Runtime.InteropServices;

namespace DynamicIsland.Platform.Hwnd;

public static class NativeMethods
{
    // Win32 sabitleri (hex değerler MSDN'den) — const, çünkü asla değişmez
    public const long WS_POPUP = 0x80000000;  // Çerçevesiz bir pencere
    public const int WS_EX_TOPMOST = 0x00000008; // Her zaman üstte
    public const int WS_EX_TOOLWINDOW = 0x00000080; // Alt-Tab da görünme
    public const int WS_EX_NOACTIVATE = 0X08000000; // Tıklayınca odak çalma

    // P/Invoke: Bu C fonksiyonu şurda yaşıyor, böyle çağıracağım beyanı.
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int index); // mevcut stili oku

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle); // stili yazar

    // index sabitleri
    public const int GWL_STYLE = -16; // pencere stili (WS_*)
    public const int GWL_EXSTYLE = -20; // genişletilmiş stili (WS_EX_*)

}
