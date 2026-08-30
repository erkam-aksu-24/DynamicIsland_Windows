# GEREKSINIMLER — Dynamic Island for Windows

> Bu belge projenin tek doğruluk kaynağıdır. Ajan (AI) ile geliştirme yapılırken buradaki
> kararlar ve kurallar esastır. Kodlama talimatları `.agent` dosyasındadır.

## 1. Vizyon

Windows için iOS tarzı "Dynamic Island": ekranın üst-ortasında yüzen, her zaman üstte duran,
animasyonlu hap (pill) şeklinde bir widget. **Fonksiyonel çekirdek:** medya kontrolü
(şarkı bilgisi, play/pause/next/prev, ses). Genişletilebilir panel yapısıyla ileride
bildirim, sayaç, panoya-kopyalandı çipi ve **dahili AI asistanı** eklenecek.

Kullanıcının birincil hedefi: **öğrenmek** — bu yüzden ajan kodu yazmaz, kullanıcı yazar.

## 2. Teknoloji Yığını (sabit kararlar)

| Karar | Değer | Gerekçe |
|---|---|---|
| Dil/Runtime | C# / .NET 8 | En yüksek öğrenme değeri, HWND + WinRT'ye doğrudan erişim |
| UI | WPF | Katmanlı/topmost pencere konusunda en derin dokümantasyon |
| Hedef TFM | `net8.0-windows10.0.19041.0` (Application katmanı saf `net8.0`) | WinRT API erişimi bir TFM uzaklıkta |
| Medya kontrolü | WinRT SMTC — `GlobalSystemMediaTransportControlsSessionManager` | Spotify/Chrome/VLC hepsiyle çalışır |
| Ses | Core Audio — `IAudioEndpointVolume` (+ v2'de `IAudioSessionManager2`) | Master + uygulama bazlı ses |
| Bildirim (v2) | `UserNotificationListener` (kullanıcı onayı gerekir) | — |
| Animasyon | WPF Storyboard + Windows.UI.Composition (spring) | iOS hissi için spring easing |

Reddedilenler: Electron, Tauri, Flutter — WinRT erişimi acı verici, öğrenme değeri düşük.

## 3. Mimari (5 katman, bağımlılık yönü değişmez)

```text
App ──► Presentation ──► Application ◄── Adapters
  │            │              ▲
  └──────────► Platform ───────┘
```

- **DynamicIsland.Application** — use case'ler, portlar (arayüzler), domain event'leri.
  **SAF KATMAN: `Windows.*`, `System.Windows`, Win32/WinRT referansı YASAK.**
  Grep kuralı: bu dosyalarda `using Windows.Media` görürsen mimari çökmüş demektir.
- **DynamicIsland.Adapters** — portların platform implementasyonları (SmtcMedia, CoreAudio,
  NotificationListener, ClipboardListener). WinRT event'leri UI thread'e marshal edilir.
- **DynamicIsland.Platform** — ham Win32/DWM yardımcıları: Hwnd/, Dpi/, Fullscreen/.
- **DynamicIsland.Presentation** — view'lar, view-model'ler, animasyon orkestrasyonu:
  Island/ (pill durumları), Panels/ (takılabilir paneller), Animations/.
- **DynamicIsland.App** — composition root (DI), Shell/ (pencere + tray), Preferences/
  (JSON ayarlar), Diagnostics/ (log + global exception handler).

## 4. Kural Seti (ajanla geliştirme kuralları)

1. **Ajan asla tam/hazır kod yazmaz.** Sadece pseudocode, küçük illüstratif parçacıklar ve
   açıklama verir. Gerçek implementasyonu kullanıcı yazar; ajan sonrasını review eder.
2. **MCP sunucularını kullanmaktan çekinme** (dokümantasyon güncel tutma vb.).
3. **Birincil yığın C# ve WPF.** Animasyonlar veya ek özellikler için ileride farklı
   diller/geliştirme ortamları kullanılabilir — kullanıcı sormadan yığın değişmez.
4. **Modülerlik + SOLID.** Proje başlangıç aşamasında; gelecekteki genişlemelere açık kalmalı.
5. **Dahili AI asistanı gelecekte gelecek.** Kapı açık tutulur: Application katmanında
   `IAiAssistant` portu + takılabilir panel deseni (`IIslandPanel`). AI = yeni adapter +
   yeni panel, asla çekirdek yeniden yazımı. **Şu an AI kodu yazılmaz.**

## 5. Faz Planı (doğrulanabilir başarı kriterleriyle)

### Faz 0 — Pencere Spike (hedef: 1-2 akşam)
- `WS_POPUP | WS_EX_TOPMOST | WS_EX_LAYERED | WS_EX_NOACTIVATE` hap pencere,
  primary monitor work-area üst-ortada, yuvarlak köşeler, tray'den göster/gizle,
  Alt-Tab'da yok (`WS_EX_TOOLWINDOW`), PerMonitorV2 DPI.
- ✅ **Kriter:** hap görev çubuğunun üstünde görünüyor, şeffaf köşeler tıklamayı yutmuyor
  (görev çubuğu saatine tıkla), ölçek 100%→150% değişince yeniden başlatmadan doğru konumda.

### Faz 1 — Medya MVP (hedef: 3-4 akşam; bunun sonunda "fonksiyonel widget" biter)
- SMTC entegrasyonu: `SessionsChanged` / `MediaPropertiesChanged` / `PlaybackInfoChanged`.
- Hap: sanatçı — şarkı + albüm kapağı. Şarkı değişince otomatik görünme, N sn sonra kapanma.
- Kontroller: play/pause, next, prev; hap üzerinde fare tekerleği = master ses.
- Açıl/kapan spring animasyonu. Ayarlar JSON (kapanma süresi, monitör, hassasiyet).
- Tek instance Mutex + tray menüsü. Use-case katmanına fake `IMediaTransport` ile unit test.
- ✅ **Kriter:** Spotify'dan Chrome YouTube'a geçiş ~1 sn'de otomatik güncellenir;
  hapdaki play/pause her iki uygulamada da çalışır; tekerlek Windows ses açılırını oynatır;
  tam ekran (borderless) uygulama açılınca hap kendini gizler.

### Faz 2 — Sağlamlık (hafta sonu işi)
- DWM acrylic/blur zemin. Çoklu oturum seçici (Spotify + Chrome aynı anda çalarken pinleme).
- Ayarlar UI'si (JSON elle düzenleme değil). Global exception handler → log → tray toast.
- Kenar durumları: animasyon ortasında DPI değişimi, expand sırasında oturum ölümü,
  kapak yükleme hatası, monitörün çekilmesi. 24 saat soak testi.
- ✅ **Kriter:** 24 saat çökme/takılı-kalma yok; monitör çekilince hap bir animasyon
  karesinde kalan monitöre yeniden demirler; Spotify ölünce hap zarifçe kapanır.

### Faz 3 — Genişleme ekosistemi (süregelen)
- Bildirim dinleyici (Settings onay akışı), panoda "Copied" çipi (opt-in, kapalı başlar),
  Pomodoro/timer paneli, `IIslandPanel` panel deseni resmîleşir.
- ✅ **Kriter:** PowerShell'den test toast'ı → hap ~2 sn içinde genişleyip gösterir;
  onay kapatılınca sessizce medya-only moda düşer, hata yok.

### Faz 4 — Dahili AI (ileri tarih; şimdilik sadece kapı açık)
- `IAiAssistant` portu + AI adapter'ı (yerel model veya API) + AI paneli.
- ✅ **Kriter:** panel takılabilir, çekirdeğe dokunmadan devre dışı bırakılabilir.

## 6. Kritik Teknik Riskler (herkesin öldüğü yerler)

| Risk | Çözüm |
|---|---|
| Şeffaf pencere tıklamaları yutar | Pencere rect = hap rect (tam genişlik şerit DEĞİL); her resize'da `SetWindowRgn` yenile |
| Tam ekran oyunlar | `SHQueryUserNotificationState` ~1 sn poll; `QUNS_RUNNING_D3D_FULL_SCREEN`/`QUNS_BUSY` → otomatik gizle |
| DPI | `PerMonitorV2` manifest; `WM_DPICHANGED` + `DisplaySettingsChanged` yakala; 100/125/150/200%'de test |
| Odak çalma | `WS_EX_NOACTIVATE` + `WM_MOUSEACTIVATE → MA_NOACTIVATE`; ham mouse input |
| Çoklu SMTC oturumu | Sıralama: playing > en son değişen > kullanıcı-pinned; Faz 2'de seçici |
| Thumbnail stream ömrü | Bir kez `System.IO.Stream`'e çevir, track ID ile cache'le, deterministik dispose |
| WinRT thread marshalling | Adapter'da `SynchronizationContext`/`TaskScheduler` yakala; view-model thread görmez |
| Event handler sızıntısı | SMTC event aboneliklerini tutarlıunsubscribe et (nesneleri pin'ler) |
| AV false positive | Global hook KULLANMA; README'de belgele; mümkünse imzala |

## 7. Altyapı ve Çalışma Akışı

- **Bellek/hafıza:** TencentDB Agent Memory (MemoryProxy + MemoryCore) üzerinden; oturum
  başlatmada takım → ajan → görev seçimi, her turda L0 write-back, Knowledge Code-Graph
  repo bağlamını sağlar.
- **Kod grafiği:** repo GitHub'a push edilir, `/v3/code-graph/create` ile bağlanır; ajan
  `explore/callers/impact` araçlarıyla yapıyı sorgular.
- **Çalışma döngüsü:** kullanıcı görevi tarif eder → ajan pseudocode + açıklama verir →
  kullanıcı kodu yazar → ajan review eder (bağlam + doğruluk) → test/build doğrulanır.
- **Doğrulama komutları:** `dotnet build DynamicIsland.sln`, `dotnet test DynamicIsland.sln`.
