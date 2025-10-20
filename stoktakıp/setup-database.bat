@echo off
chcp 65001 > nul
echo ================================
echo Envanter Yönetim Sistemi
echo Veritabaný Kurulum Script'i
echo ================================
echo.

cd stoktakýp

echo 1. NuGet paketleri yükleniyor...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo HATA: NuGet paketleri yüklenemedi!
    pause
    exit /b 1
)
echo Baþarýlý: NuGet paketleri yüklendi.
echo.

echo 2. Proje derleniyor...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Proje derlenemedi!
    pause
    exit /b 1
)
echo Baþarýlý: Proje derlendi.
echo.

echo 3. EF Core Tools kontrol ediliyor...
dotnet tool list -g | findstr "dotnet-ef" > nul
if %ERRORLEVEL% NEQ 0 (
    echo EF Core Tools yükleniyor...
    dotnet tool install --global dotnet-ef
)
echo Baþarýlý: EF Core Tools hazýr.
echo.

echo 4. Migration oluþturuluyor...
dotnet ef migrations add InitialCreate
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Migration oluþturulamadý!
    pause
    exit /b 1
)
echo Baþarýlý: Migration oluþturuldu.
echo.

echo 5. Veritabaný oluþturuluyor...
dotnet ef database update
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Veritabaný oluþturulamadý!
    pause
    exit /b 1
)
echo Baþarýlý: Veritabaný oluþturuldu.
echo.

echo ================================
echo KURULUM TAMAMLANDI!
echo ================================
echo.
echo Uygulamayý çalýþtýrmak için:
echo   dotnet run
echo.
echo veya Visual Studio'da F5 tuþuna basýn.
echo.
pause

