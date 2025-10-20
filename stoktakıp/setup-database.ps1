# Envanter Yönetim Sistemi - Veritabanı Kurulum Script'i

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Envanter Yönetim Sistemi" -ForegroundColor Cyan
Write-Host "Veritabanı Kurulum Script'i" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Proje dizinine git
$projectPath = Join-Path $PSScriptRoot "stoktakıp"
Set-Location $projectPath

Write-Host "1. NuGet paketleri yükleniyor..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "HATA: NuGet paketleri yüklenemedi!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ NuGet paketleri başarıyla yüklendi." -ForegroundColor Green
Write-Host ""

Write-Host "2. Proje derleniyor..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "HATA: Proje derlenemedi!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Proje başarıyla derlendi." -ForegroundColor Green
Write-Host ""

Write-Host "3. EF Core Tools kontrol ediliyor..." -ForegroundColor Yellow
$efToolInstalled = dotnet tool list -g | Select-String "dotnet-ef"
if (-not $efToolInstalled) {
    Write-Host "EF Core Tools bulunamadı, yükleniyor..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "✓ EF Core Tools başarıyla yüklendi." -ForegroundColor Green
} else {
    Write-Host "✓ EF Core Tools zaten yüklü." -ForegroundColor Green
}
Write-Host ""

Write-Host "4. Migration oluşturuluyor..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate
if ($LASTEXITCODE -ne 0) {
    Write-Host "HATA: Migration oluşturulamadı!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Migration başarıyla oluşturuldu." -ForegroundColor Green
Write-Host ""

Write-Host "5. Veritabanı oluşturuluyor ve seed data yükleniyor..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "HATA: Veritabanı oluşturulamadı!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Veritabanı başarıyla oluşturuldu." -ForegroundColor Green
Write-Host ""

Write-Host "================================" -ForegroundColor Cyan
Write-Host "KURULUM TAMAMLANDI!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Uygulamayı çalıştırmak için:" -ForegroundColor Yellow
Write-Host "  dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "veya Visual Studio'da F5 tuşuna basın." -ForegroundColor White
Write-Host ""
Write-Host "Uygulama https://localhost:7xxx adresinde açılacaktır." -ForegroundColor Cyan
Write-Host ""

