# Envanter Yönetim Sistemi

Bilgi işlem departmanı ekipmanlarının takibini sağlayan kapsamlı bir ASP.NET Core MVC web uygulaması.

## Özellikler

### 1. Cihaz Yönetimi
- ✅ Cihaz ekleme, düzenleme, silme, listeleme
- ✅ Cihaz detayları ve işlem geçmişi görüntüleme
- ✅ Benzersiz seri numarası kontrolü
- ✅ Marka, model ve stok bilgileri yönetimi

### 2. Teslim İşlemleri
- ✅ Stok giriş işlemleri
- ✅ Personele teslim (çıkış) işlemleri
- ✅ İade işlemleri
- ✅ Teslim eden ve teslim alan kişi takibi
- ✅ Otomatik stok güncelleme
- ✅ İşlem geçmişi ve filtreleme

### 3. Kapsamlı Raporlama Modülü
- ✅ **Dashboard**: Genel bakış, istatistikler, hızlı erişim
- ✅ **Mevcut Stok Durumu Raporu**: Tüm cihazların güncel stok bilgileri
- ✅ **Teslim Geçmişi Raporu**: Kronolojik işlem listesi, filtreleme
- ✅ **Kişiye Göre Zimmetli Cihazlar**: Personel bazlı cihaz listesi
- ✅ **Cihaz Bazlı Hareket Raporu**: Cihaz bazında giriş-çıkış geçmişi
- ✅ **Marka/Model Dağılım Raporu**: İstatistiksel dağılım analizi
- ✅ **İade Edilmemiş Cihazlar**: Zimmetli cihazların takibi

### 4. Excel Export
- ✅ Tüm raporlar Excel formatında indirilebilir
- ✅ ClosedXML kütüphanesi kullanılarak profesyonel Excel çıktıları

### 5. Validasyon ve Güvenlik
- ✅ Model validasyonu (Data Annotations)
- ✅ Seri numarası benzersizlik kontrolü
- ✅ Yetersiz stok kontrolü
- ✅ Teslim/iade işlemlerinde stok kontrolü
- ✅ Foreign key ilişkileri ve veri bütünlüğü

### 6. Modern UI/UX
- ✅ Bootstrap 5 ile responsive tasarım
- ✅ Font Awesome ikonları
- ✅ Card tabanlı modern arayüz
- ✅ Animasyonlu alert mesajları
- ✅ Progress bar ile stok görselleştirme
- ✅ Hover efektleri ve geçişler
- ✅ Yazdırma için optimize edilmiş sayfa tasarımları

## Teknolojiler

- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: SQL Server / LocalDB
- **Frontend**: Bootstrap 5, Font Awesome 6, jQuery
- **Export**: ClosedXML (Excel), iTextSharp (PDF hazır)

## Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8.0 SDK
- SQL Server veya SQL Server Express / LocalDB
- Visual Studio 2022 veya VS Code

### Adım 1: NuGet Paketlerini Yükleme
```bash
cd stoktakıp\stoktakıp
dotnet restore
```

### Adım 2: Veritabanı Migration
```bash
# Migration oluşturma
dotnet ef migrations add InitialCreate

# Veritabanını oluşturma ve seed data'yı yükleme
dotnet ef database update
```

**Not**: Eğer `dotnet ef` komutu bulunamazsa:
```bash
dotnet tool install --global dotnet-ef
```

### Adım 3: Projeyi Çalıştırma
```bash
dotnet run
```

Veya Visual Studio'da **F5** tuşuna basarak çalıştırın.

Uygulama şu adreste açılacaktır: `https://localhost:7xxx` (port numarası değişebilir)

## Veritabanı Yapısı

### Cihazlar Tablosu
- `CihazId` (PK): Benzersiz kimlik
- `CihazAdi`: Cihaz adı (Laptop, Mouse, vb.)
- `Marka`: Cihaz markası
- `Model`: Cihaz modeli
- `SeriNumarasi`: Benzersiz seri numarası (Unique Index)
- `ToplamAdet`: Toplam cihaz adedi
- `MevcutStok`: Depodaki mevcut stok
- `KayitTarihi`: Kayıt oluşturulma tarihi

### TeslimIslemleri Tablosu
- `IslemId` (PK): Benzersiz kimlik
- `CihazId` (FK): Cihaz referansı
- `TeslimEden`: Teslim eden kişi adı
- `TeslimAlan`: Teslim alan kişi adı
- `TeslimTarihi`: Teslim tarihi
- `IadeTarihi`: İade tarihi (nullable)
- `IslemTipi`: Enum (StokGirisi, PersoneleTeslim, Iade)
- `Adet`: İşlem adedi
- `Aciklama`: İsteğe bağlı açıklama

### İlişkiler
- Cihazlar ↔ TeslimIslemleri (One-to-Many)
- Foreign Key: `TeslimIslemleri.CihazId` → `Cihazlar.CihazId`
- Delete Behavior: Restrict (Cihaz silinemez eğer teslim işlemi varsa)

## Seed Data

Uygulama ilk çalıştırıldığında otomatik olarak örnek cihazlar eklenir:
1. Dell Latitude 5420 Laptop (10 adet)
2. Logitech M185 Mouse (20 adet)
3. Logitech K120 Klavye (15 adet)

## Sayfa Yapısı

### Ana Menü
- **Dashboard**: Genel bakış ve istatistikler
- **Cihazlar**: Cihaz CRUD işlemleri
- **Teslim İşlemleri**: Stok giriş/çıkış işlemleri
- **Raporlar**: 7 farklı detaylı rapor

### Cihazlar Modülü
- Index: Cihaz listesi, arama
- Create: Yeni cihaz ekleme
- Edit: Cihaz düzenleme
- Details: Cihaz detayları ve işlem geçmişi
- Delete: Cihaz silme (eğer teslim işlemi yoksa)

### Teslim İşlemleri Modülü
- Index: İşlem listesi, filtreleme (tarih, kişi)
- Create: Yeni işlem oluşturma
- Details: İşlem detayları
- IadeEt: Cihaz iade işlemi
- Delete: İşlem silme (stok düzeltmesiyle)

### Raporlar Modülü
- Dashboard: İstatistikler, son işlemler, düşük stok uyarıları
- MevcutStokDurumu: Tüm cihazların stok durumu
- TeslimGecmisi: Tarih bazlı işlem geçmişi
- KisiyeGoreZimmetli: Personel bazında zimmetli cihazlar
- CihazBazliHareket: Cihaz bazında hareket geçmişi
- MarkaModelDagilim: Marka ve model dağılım istatistikleri
- IadeEdilmemis: Henüz iade edilmemiş cihazlar

## İş Kuralları

### Stok Kontrolü
- Personele teslim işleminde yetersiz stok varsa işlem engellenenir
- Stok girişi toplam adet ve mevcut stoku artırır
- Personele teslim mevcut stoku azaltır
- İade işlemi mevcut stoku artırır

### Validasyon Kuralları
- Seri numarası benzersiz olmalıdır
- Cihaz adı, marka, model zorunludur
- Toplam adet en az 1 olmalıdır
- Teslim eden ve teslim alan kişi bilgileri zorunludur
- Tarih alanları geçerli tarih formatında olmalıdır

### Silme Kuralları
- Teslim işlemi olan bir cihaz silinemez
- Teslim işlemi silindiğinde stok otomatik düzeltilir

## Özelleştirme

### Connection String
`appsettings.json` dosyasında connection string'i düzenleyebilirsiniz:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EnvanterDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### CSS Özelleştirme
`wwwroot/css/site.css` dosyasında özel stil tanımlamaları yapabilirsiniz.

## Ekran Görüntüleri

### Dashboard
- İstatistik kartları (Toplam cihaz, stok, kullanımda, iade bekleyen)
- Hızlı işlem butonları
- Son 10 işlem listesi
- Düşük stoklu cihazlar uyarısı
- Rapor bağlantıları

### Raporlar
- Filtreleme ve arama özellikleri
- Excel export butonları
- Yazdırma desteği
- Detaylı tablolar ve istatistikler
- Progress bar ile görsel stok göstergesi

## Geliştirme Notları

### Gelecek Özellikler (İsteğe Bağlı)
- PDF export özelliği (iTextSharp.LGPLv2.Core kütüphanesi yüklü)
- Grafik ve görselleştirmeler (Chart.js entegrasyonu)
- E-posta bildirimleri (düşük stok, uzun süreli zimmet)
- Kullanıcı yetkilendirme (ASP.NET Identity)
- API endpoint'leri (RESTful API)
- Cihaz resimleri yükleme
- QR kod ile cihaz takibi

## Lisans

Bu proje eğitim amaçlıdır.

## Destek

Sorun bildirimleri ve öneriler için GitHub Issues kullanabilirsiniz.

---

**Geliştirici Notu**: Proje tüm istenen özellikleri içermektedir. Migration oluşturup veritabanını güncelledikten sonra kullanıma hazırdır.

