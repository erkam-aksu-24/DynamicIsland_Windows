# WORKLOG — Dynamic Island for Windows

> Her çalışma oturumunun sonunda günlük girdi eklenir (en yenisi üstte).
> Amaç: AI hafızası silinse bile proje tarihi kodun yanında kalsın.
> Ayrıntılı gereksinimler için: `GEREKSINIMLER.md` — kodlama kuralları için: `.agent`

## 2026-10-13 — F turu: Faz 1 KAPATILDI 🏁 (2 erteleme notuyla)

**F. Kapanış kriterleri turu ✅**
- Kriter 1 (Spotify→Chrome ~1 sn): **elle test ✅** — geçiş "saniyesinde"; MediaOrchestrator
  aktif oturum seçimi + track cache/replay (E) bu hızı sağlıyor
- Kriter 3 (tekerlek = master ses): **canlı test ✅** (2026-10-13 D girdisi)
- Kriter 2 (play/pause iki uygulamada): **kısmi** — komut hattı (TogglePlayPauseHandler + unit
  test, Tur C) kanıtlı; ama hap/tray UI'sında play/pause butonu YOK (tray yalnız Göster/Gizle +
  Çıkış — kod incelemesiyle tespit edildi). UI bağlantısı Faz 2'ye ertelendi
- Kriter 4 (tam ekran gizlenme): kod hiç yazılmamıştı (Platform README'de planlı); karar:
  **Faz 2'ye resmen taşındı** (zaten §6 risk tablosunda SHQueryUserNotificationState ile yerli yeri)
- Sonuç: **Faz 1 kapatıldı** — "fonksiyonel widget" çekirdeği canlı (şarkı bilgisi + otomatik
  aç/kapa + ayar dosyası + ses tekerleği). İki dürüst erteleme: play/pause UI, tam ekran gizlenme

**Öğrenilen:** kapanış kriteri turu kod yazma değil KANIT toplama işi; "kısmi"yi kapatırken dürüst
belgelemek, yeşil-yeşil göstermekten değerli — Faz 2 backlog'u iki somut kalemle başlıyor

**Faz 2 backlog'a aktarılanlar (GEREKSINIMLER.md'den + yeni):** acrylic, oturum seçici, ayarlar
UI, play/pause butonu (hap + tray), tam ekran gizlenme (SHQueryUserNotificationState),
IMMNotificationClient (cihaz değişimi), soak testi

## 2026-10-13 — Faz1 D tamamlandı: ses tekerleği canlı 🎉

**D. Ses tekerleği ✅**
- `CoreAudioController` (Adapters/CoreAudio) — `IAudioController`'ın ilk gerçek implementasyonu
  - COM zinciri: MMDeviceEnumerator → GetDefaultAudioEndpoint(eRender=0, eConsole=0) →
    Activate(IID_IAudioEndpointVolume) → Get/SetMasterVolumeLevelScalar (0.0–1.0 skaler — port
    sözleşmesiyle birebir örtüşme, yalnız float↔double dönüşümü adapter sınırında)
  - COM sözleşmeleri [ComImport] gölge arayüzlerle; vtable gap tekniği
    (_VtblGap1_4 / _VtblGap1_1 — sayılar resmi endpointvolume.h'dan: Set'ten önce 4, Get'ten önce 1)
  - Hata politikası: **last-known cache** — ctor/Get/Set try-catch + Debug.WriteLine (fail-soft);
    endpoint çözülemezse null kalır (kör mod), cache'e yalnızca doğrulanmış değer girer
  - `eConsole = 0` — IDL doğrulaması: ERole {eConsole=0, eMultimedia=1, eCommunications=2};
    ilk yazımda 1'di (eMultimedia olurdu), canlı test bunu FARK ETMEZ çünkü çoğu sistemde iki rol
    aynı cihazı gösterir — rol-özgü cihaz seçiminde yanlış cihaza yazardı
- IslandWindow: `OnPreviewMouseWheel` **override** (Yol B) — XAML attribute yerine override deseni,
  base çağrısıyla; çentik = `e.Delta / 120.0` (serbest dönüşlü farede kesirli adım korunur)
  × `WheelVolumeStep` → `AdjustVolumeHandler.Execute`; `e.Handled = true`; tekerlekte timer restart
- DI (+2 satır): `IAudioController→CoreAudioController` + `AdjustVolumeHandler`
- `JsonPreferencesStore`: CS8603 → `settings ?? new IslandSettings()` — WORKLOG'un "null→defaults"
  4. dalı kodda eksikti (WORKLOG 4 dal diyordu, kod 3 daldı); nullable uyarı sürüklenmeyi yakaladı
- Doğrulandı: build 0 uyarı/0 hata; test **8/8**; canlı tur — **odak başka uygulamadayken** hap
  üzerinde tekerlek ses açılırını oynattı ✅ (WM_MOUSEWHEEL Win10+ imleç-altı kuralı teyit edildi;
  NOACTIVATE pencere fare/tekerlek girdisi alıyor; HwndSource.AddHook yedeğine gerek kalmadı)

**Öğrenilen:**
- `[ComImport]` coclass + `sealed` → **CS0030**: derleyici sealed sınıfı arayüze cast'e izin vermez
  (alt sınıf ihtimali kapanınca COM QI yolu görünmez). Coclass'lar sealed yapılmaz. Kanıt deneyiyle
  bulundu: birebir kopya izole proje → aynı hata; sealed kaldır → build temiz + `dotnet run` True
  (CoCreateInstance gerçekten ateşlendi)
- CS8603 nullable uyarısı = "belgelenen niyet ↔ kod" sürüklenme dedektörü; `"null"` geçerli
  JSON'dur ve catch'e düşmez → `??` ile 4. dal tamamlandı
- WM_MOUSEWHEEL tarihi (Raymond Chen): 1996'dan beri odak penceresine gider (klavye durumu
  kuyruğa bağlıydı); Win10+ pencere yöneticisi imleç-altına kırıyor; NOACTIVATE pencere
  WM_MOUSEACTIVATE'e rağmen fare girdisini alır (XMouse kenar durumu → Faz 2 notu)
- ComImport cast = QueryInterface; retval dönüşümü: native `float* pfLevel` out'u C# dönüş
  değerine eşlenir (PreserveSig'siz konvansiyon)
- Sabit-ad disiplini: adı `eConsole` olan sabitin değeri 1 ise kod yalan söyler; ve
  **sabitleri kendi gözünle belgelemeden onaylatma** — mentor onayı hatalıydı, SDK IDL teyit etti

**Bilinen sınırlama:** ses cihazı değişimi takip edilmiyor (IMMNotificationClient → Faz 2);
Sndvol'den dış değişiklik callback'siz cache'i bayatlatmaz (handler her seferinde taze GetVolume).

**Faz 1 durum özeti:** A✅ B✅ C✅ D✅ E✅ 5✅ — kalan: **F (kapanış kriteri turu)**

## 2026-10-12 — Faz1 Adım 5 tamamlandı: ayar dosyası hattı canlı 🎉

**5. Ayar bağlama ✅**
- `JsonPreferencesStore` (App/Preferences) — `IUserPreferences`'ın ilk gerçek implementasyonu
  - Yol: `%APPDATA%\DynamicIsland\settings.json` (GetFolderPath + Path.Combine, hardcode yok)
  - Ctor: klasör garantisi (CreateDirectory idempotent — Exists kontrolü gereksiz, silindi)
  - Load: 4 dal — dosya yok→defaults; sağlam→deserialize; null→defaults; bozuk JSON→defaults
  - Save: serialize + yazma; hata → Debug.WriteLine (sessiz, çökme yok)
- DI: `AddSingleton<IUserPreferences, JsonPreferencesStore>()` (arayüz→somut doğrudan desen)
- IslandWindow: ctor'a IUserPreferences enjeksiyonu; `_collapseSeconds` field'ı — **tek okuma
  ctor'da**, sıcak yolda I/O yok (her şarkı değişiminde disk okuması engellendi)
- Doğrulandı (manuel): A) dosya yok→5sn varsayılan; B) elle `{ "CollapseAfterSeconds": 12 }`
  → hap 12 sn açık kalmalı ✅ (kullanıcı sözleşmesi canlı); C) bozuk JSON→çökme yok, 5sn ✅
- Öğrenilen: %APPDATA% ortam değişkeni ↔ SpecialFolder.ApplicationData; idempotent API;
  Nullable sözleşme uyumu (port `IslandSettings` derken implementasyon `IslandSettings?`
  olmaz — zayıf söz CS8602); sıcak yolda I/O kaldırmama; catch+throw anti-pattern

**Faz 1 durum özeti:** A✅ B✅ C✅ E✅ 5✅ — kalan: **D (ses tekerleği)**, F (kapanış kriteri turu)

## 2026-10-11 — Faz1 C2 tamamlandı: otomatik aç/kapa 🎉 (Faz 1 fonksiyonel çekirdek bitti)

**C2. Otomatik aç/kapa ✅**
- `IslandViewModel`a `ShowRequested` event'i (VM karar verir: farklı şarkı mı? —
  `_lastShownTitle` karşılaştırması; aynı şarkı replay'de yeniden açılmaz)
- `IslandWindow`: `DispatcherTimer` + `vm.ShowRequested` aboneliği —
  `Show()` + `Interval=5sn` + `Start()` (Start zaten restart eder); `Tick` → `Hide()+Stop()`
- Doğrulandı (manuel): şarkı değişince hap açılır, ~5 sn sonra kendiliğinden kapanır;
  aynı şarkıda pause/play hapı yeniden açmaz; build 0 hata, test 8/8
- Öğrenilen: DispatcherTimer UI-thread nesnesidir → VM'e konmaz (VM saf kalır);
  "karar VM'de, uygulama pencerede" sorumluluk zinciri; Start()=restart davranışı
- Bekleyen TODO: `5` sabit — `IslandSettings.CollapseAfterSeconds`'tan gelecek
  (JsonPreferencesStore iskeleti hazır ama NotImplementedException — sıradaki iş)

**Faz 1 durum özeti:** A✅ B✅ C✅(C1 binding + C2 timer) E✅ — kalan: D (ses tekerleği),
5→ayar bağlama, F (kapanış kriteri turu). "Fonksiyonel widget" hedefi vizyonda!

---

## 2026-10-07 — Faz1: B + C(1/2) + E tamamlandı; hap canlı 🎉

**B. ViewModel hattı ✅**
- `ObservableObject` (Presentation/Island) — INPC tabanı, `[CallerMemberName]`
- `IMediaFeed` portu (Application/Ports) — orchestrator'ın 3 event'i resmileşti (DIP)
- `IslandViewModel : ObservableObject` — 3 event aboneliği + Title/Artist/IsPlaying
- DI: `AddSingleton<IslandViewModel>` + `IMediaFeed` → orchestrator factory
- Öğrenilen: handler'da `var` shadowing tuzağı (local ≠ property); DI dairesel bağımlılık
  (VM ctor'unun kendisini istemesi → InvalidOperationException; DataContext VM'in değil
  pencerenin özelliğidir)

**C. UI bağlama (1/2 — binding kısmı) ✅**
- `IslandWindow(IslandViewModel)` ctor enjeksiyonu + `DataContext = vm`
- XAML: Border > StackPanel(Horizontal) > TextBlock'lar, `{Binding Artist}` / `{Binding Title}`
- Genişlik kararı: sabit 220 + `TextTrimming` (iOS hapı davranışı)
- Öğrenilen: yatay StackPanel'de trimming tek başına ÇALIŞMAZ (sonsuz ölçü kısıtı) →
  `MaxWidth` gereklidir; DataContext DI yönetiliyorsa code-behind'da set edilir,
  XAML nesne yaratmaz
- İLK CANLI TEST ✅: Spotify çalarken hap üzerinde sanatçı—şarkı görünüyor
- Bilinen bug (C1'de bulundu, E ile çözüldü): app geç başlatılırsa ilk track yutulur
  (LoadTrackInfo async yarışı, aktif seçimden önce gelir → filtre yutar)

**E. Geç gelen şarkı düzeltmesi ✅**
- `MediaOrchestrator`a `_trackCache` (oturum başına son TrackInfo)
- Aktif oturum değişince cache'ten replay; aktif olmayan oturumların track'i artık
  kaybolmaz
- Budama (prune): `OnSessionsChanged`'te canlı oturumlarla senkronizasyon —
  iki fazlı desen (ölüleri topla → sil; gezinirken değiştirme tuzağı)
- Testler (+2): `OnTrackChanged_Cache_Test.cs` — replay pozitif + hayalet negatif;
  `Assert.DoesNotContain` spesifik negatif kanıt; trackA = kontrollü test verisi
- Doğrulandı: `dotnet test` → 8/8 ✅

**Sıradaki:** C(2/2) — track gelince hap göster + `CollapseAfterSeconds` sonra kapa
(DispatcherTimer); D — fare tekerleği ses; F — Faz 1 kapanış kriteri turu

---

## 2026-09-29 — Hafıza kurtarma + durum denetimi + yol haritası

**Ne oldu:**
- Eski AI hafızası (TDAI L0/L1/L2) profil taşıması nedeniyle silindi.
- Kurtarma araması yapıldı: git geçmişi sağlam, GEREKSINIMLER.md sağlam.
- Eski OpenCode oturum kayıtları kurtarılamadı (boş dosyalar kaldı).
- 9 commit'in tamamı GitHub'a push edildi (son 6 commit push'lanmamıştı!).

**Durum denetimi (build + test):**
- `dotnet build` ✅ 0 hata / `dotnet test` ✅ 6/6 geçti.

**Kod inceleme bulguları (henüz düzeltilmedi):**
1. `App.xaml.cs`: Mutex kontrolü en sonda → ikinci instance tray/pencere kurduktan sonra kapanıyor. İlk iş olmalı.
2. `App.xaml.cs`: IslandWindow iki kez `Show()` ediliyor (satır 16 ve 29).
3. `App.xaml.cs`: `GetRequiredKeyedService<SmtcMediaController>(null)` → keyed kayıt yok, runtime'da fırlatır. `GetRequiredService` olmalı.
4. `MediaOrchestrator`: aktif olmayan oturumların track bilgilerini yutuyor → oturum değişince VM şarkıyı bilemez. Çözüm: oturum başına TrackInfo cache + aktifleşince yayın.

**Commit edilmemiş işler (working tree):**
- `SkipNextHandler` / `SkipPreviousHandler` (Application/UseCases) — review bekliyor
- `TrayIconHost.cs` değişikliği — diff incelenmeli
- `IslandViewModel` / `RelayCommand` — boş stub'lar

**Sonraki adımlar:** aşağıdaki Yol Haritası bölümüne taşındı.

---

## Yol Haritası (Faz 1'i kapatmaya odaklı)

Öncül: GEREKSINIMLER.md Faz 1 kriterleri esas. Sıra bağımlılıklara göre:

- [x] **A. Temizlik turu** (2026-10-05 tamamlandı)
  - ~~SkipNext/SkipPrevious handler'larını review et + commit et~~ (kullanıcı commit etti: 89f1fae)
  - ~~TrayIconHost diff'ini incele~~ (kullanıcı commit etti)
  - ~~App.xaml.cs'deki 3 kusuru düzelt (mutex sırası, çift Show, keyed service)~~ (kullanıcı yazdı, review: OK)
  - Doğrulandı: build temiz, test 6/6, ikinci instance flaşsız sessizce kapanıyor (manuel test ✅)
  - Not: GetRequiredKeyedService(null) crash ETMEZ (empirik test edildi, null → non-keyed fallback); düzeltme netlik içindi
- [x] **B. ViewModel hattı** (2026-10-06/07 tamamlandı — üstteki 2026-10-07 girdisine bak)
- [x] **C. UI bağlama + otomatik aç/kapa** (2026-10-11 tamamlandı — üstteki girdilere bak)
  - C1: binding + MaxWidth/trimming ✅; C2: ShowRequested + DispatcherTimer ✅
- [x] **5. Ayar dosyası hattı** (2026-10-12 tamamlandı — üstteki girdiye bak)
  - JsonPreferencesStore + DI + pencerede tek-okuma; 5→CollapseAfterSeconds ✅
- [x] **D. Ses tekerleği** (2026-10-13 tamamlandı — üstteki girdiye bak)
  - CoreAudioController (Core Audio COM zinciri) + OnPreviewMouseWheel override → AdjustVolumeHandler
  - Doğrulandı: canlı tur — odak başka uygulamada iken tekerlek ses açılırını oynattı; 8/8 yeşil
- [x] **E. Geç gelen şarkı düzeltmesi** (2026-10-07 tamamlandı — üstteki girdiye bak)
  - Orchestrator'a oturum başına TrackInfo cache; aktifleşince replay
  - Budama + hayalet testi; 8/8 yeşil
- [x] **F. Faz 1 kapanış kriteri turu** (2026-10-13 tamamlandı — üstteki girdiye bak)
  - Kriter 1 + 3 kanıtlı; kriter 2 kısmi (UI Faz 2'ye), kriter 4 Faz 2'ye taşındı
  - **Faz 1 kapatıldı** — Faz 2'ye geçildi

Faz 1 kapanınca Faz 2'ye (acrylic, oturum seçici, ayarlar UI, soak testi) geçilir.

---

<!--
## 2026-09-20 — SMTC Tur B + Tur C (geriye dönük, commit'lerden yeniden kuruldu)
- Tur A (9b531bf): SMTC adapter oturum izleme + sink sınır kapısı
- Tur B (547ea32): track bilgisi yükleme (MediaPropertiesChanged → TryGetMediaPropertiesAsync)
- Tur C (b7e45c2): transport komutları (toggle/next/prev) + tray testi

## 2026-09-16 — Faz 0 2/2 (40f0ec4)
- Tray ikonu (göster/gizle/çıkış) + tek-instance Mutex

## 2026-09-12 — Faz 0: hap penceresi (ae1fc51)
- IslandWindow: topmost, transparent, toolwindow, noactivate, PerMonitorV2
- Application çekirdek testleri (6/6)

## 2026-09-11 — Faz 1 çekirdek (5063557)
- Portlar: IMediaTransport, IMediaEventSink, IAudioController, IUserPreferences
- MediaOrchestrator (aktif oturum seçimi: playing > en son değişen)

## 2026-08-30 — Dokümantasyon (0591551)
- GEREKSINIMLER.md + .agent kuralları

## 2026-08-29 — Initial commit (d6f3b6b)
- 5 katmanlı çözüm iskeleti
-->
