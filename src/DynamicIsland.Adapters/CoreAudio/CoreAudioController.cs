using System.Diagnostics;
using System.Runtime.InteropServices;
using DynamicIsland.Application.Ports;

namespace DynamicIsland.Adapters.CoreAudio;

// --- COM sözleşmeleri (GUID'ler resmi endpointvolume.h / mmdeviceapi.h'den) ---

[ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]   // CLSID_MMDeviceEnumerator
internal class MMDeviceEnumerator { }

[ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),    // IID_IMMDeviceEnumerator
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceEnumerator
{
    void _VtblGap1_1();                                      // EnumAudioEndpoints atlanır
    [return: MarshalAs(UnmanagedType.Interface)]
    IMMDevice GetDefaultAudioEndpoint(int dataFlow, int role);   // eRender=0, eConsole=0
}

[ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"),    // IID_IMMDevice
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
    // native: HRESULT Activate(REFIID, DWORD, PROPVARIANT*, void** retval)
    [return: MarshalAs(UnmanagedType.Interface)]
    object Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams);
}

[ComImport, Guid("5CDF2C82-841E-4546-9722-0CF74078229A"),    // IID_IAudioEndpointVolume
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolume
{
    void _VtblGap1_4();                                      // Register/Unregister/GetChannelCount/SetMasterVolumeLevel(db)
    void SetMasterVolumeLevelScalar(float fLevel, IntPtr pguidEventContext);  // HRESULT→exception
    void _VtblGap1_1();                                      // GetMasterVolumeLevel(db) atlanır
    float GetMasterVolumeLevelScalar();                      // pfLevel retval konumunda → dönüş değeri olur
}

public class CoreAudioController : IAudioController
{
    //Alan
    private readonly IAudioEndpointVolume? _audioEndpointVolume;
    private double _lastKnownVolume;
    private const int CLSCTX_INPROC_SERVER = 0x1;
    private const int eRender = 0;
    private const int eConsole = 0;

    public CoreAudioController()
    {
        try
        {
            var enumerator = (IMMDeviceEnumerator) new MMDeviceEnumerator();

            var device = enumerator.GetDefaultAudioEndpoint(eRender, eConsole);
            var endpointObj = device.Activate(
                new Guid("5CDF2C82-841E-4546-9722-0CF74078229A"),  // IID_IAudioEndpointVolume
                CLSCTX_INPROC_SERVER,   // = 0x1 → private const
                IntPtr.Zero);

            _audioEndpointVolume = (IAudioEndpointVolume) endpointObj;

            _lastKnownVolume = _audioEndpointVolume.GetMasterVolumeLevelScalar();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[CoreAudio] başlatma hatası: {e}");
        }


    }

    public double GetVolume()
    {
        if (_audioEndpointVolume == null)
        {
            return _lastKnownVolume;
        }

        try
        {
            float volume = _audioEndpointVolume.GetMasterVolumeLevelScalar();
            _lastKnownVolume = volume;
            return volume;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[CoreAudio] ses seviyesi alma hatası: {e}");
            return _lastKnownVolume;
        }
    }

    public void SetVolume(double volume)
    {
        if (_audioEndpointVolume == null)
        {
            Debug.WriteLine($"[CoreAudio] ses seviyesi ayarlama hatası: _audioEndpointVolume null");
            return;
        }

        try
        {
            _audioEndpointVolume.SetMasterVolumeLevelScalar((float)volume, IntPtr.Zero);
            _lastKnownVolume = volume;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[CoreAudio] ses seviyesi ayarlama hatası: {e}");
        }
    }
}
