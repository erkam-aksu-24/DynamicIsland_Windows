# WORKLOG — Dynamic Island for Windows

> Her çalışma oturumunun sonunda günlük girdi eklenir (en yenisi üstte).
> Amaç: AI hafızası silinse bile proje tarihi kodun yanında kalsın.
> Ayrıntılı gereksinimler için: `GEREKSINIMLER.md` — kodlama kuralları için: `.agent`

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

- [ ] **A. Temizlik turu**
  - SkipNext/SkipPrevious handler'larını review et + commit et
  - TrayIconHost diff'ini incele
  - App.xaml.cs'deki 3 kusuru düzelt (mutex sırası, çift Show, keyed service)
  - Doğrula: build temiz, ikinci instance anında sessizce kapanıyor
- [ ] **B. ViewModel hattı**
  - `IslandViewModel`: INotifyPropertyChanged + orchestrator'ın 3 event'ine abonelik
  - DI wiring: VM'i orchestrator'a bağla
  - Doğrula: mevcut 6 test bozulmaz; Debug ile event akışı izlenir
- [ ] **C. UI bağlama + otomatik aç/kapa**
  - XAML: track bilgisi + play durumuna `{Binding}`
  - Track değişince göster, `CollapseAfterSeconds` sonra kapa (DispatcherTimer)
  - Doğrula: Spotify'da şarkı değişince hap güncellenip ~5 sn'de kapanır
- [ ] **D. Ses tekerleği**
  - Hap üzerinde `PreviewMouseWheel` → `AdjustVolumeHandler`
  - Doğrula: tekerlek Windows ses açılırını oynatır (IslandSettings.WheelVolumeStep)
- [ ] **E. Geç gelen şarkı düzeltmesi**
  - Orchestrator'a oturum başına TrackInfo cache; aktifleşince yayınla
  - Doğrula: yeni unit test — pasif oturumun track'i cache'lenir, aktifleşince event gelir
- [ ] **F. Faz 1 kapanış kriteri turu**
  - GEREKSINIMLER.md §5 Faz 1 ✅ kriterlerini elle test et (Spotify↔Chrome geçiş ~1 sn, play/pause iki yönlü, tekerlek, tam ekran gizlenme — son madde Faz 2'ye ertelenebilir)

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
