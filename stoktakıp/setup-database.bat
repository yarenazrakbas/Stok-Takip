@echo off
chcp 65001 > nul
echo ================================
echo Envanter Y�netim Sistemi
echo Veritaban� Kurulum Script'i
echo ================================
echo.

cd stoktak�p

echo 1. NuGet paketleri y�kleniyor...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo HATA: NuGet paketleri y�klenemedi!
    pause
    exit /b 1
)
echo Ba�ar�l�: NuGet paketleri y�klendi.
echo.

echo 2. Proje derleniyor...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Proje derlenemedi!
    pause
    exit /b 1
)
echo Ba�ar�l�: Proje derlendi.
echo.

echo 3. EF Core Tools kontrol ediliyor...
dotnet tool list -g | findstr "dotnet-ef" > nul
if %ERRORLEVEL% NEQ 0 (
    echo EF Core Tools y�kleniyor...
    dotnet tool install --global dotnet-ef
)
echo Ba�ar�l�: EF Core Tools haz�r.
echo.

echo 4. Migration olu�turuluyor...
dotnet ef migrations add InitialCreate
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Migration olu�turulamad�!
    pause
    exit /b 1
)
echo Ba�ar�l�: Migration olu�turuldu.
echo.

echo 5. Veritaban� olu�turuluyor...
dotnet ef database update
if %ERRORLEVEL% NEQ 0 (
    echo HATA: Veritaban� olu�turulamad�!
    pause
    exit /b 1
)
echo Ba�ar�l�: Veritaban� olu�turuldu.
echo.

echo ================================
echo KURULUM TAMAMLANDI!
echo ================================
echo.
echo Uygulamay� �al��t�rmak i�in:
echo   dotnet run
echo.
echo veya Visual Studio'da F5 tu�una bas�n.
echo.
pause

