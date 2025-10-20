using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stoktakýp.Data;
using stoktakýp.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;

namespace stoktakýp.Controllers
{
    public class RaporController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RaporController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Rapor Ana Sayfasý
        public IActionResult Index()
        {
            return View();
        }

        // 1. Mevcut Stok Durumu Raporu
        public async Task<IActionResult> MevcutStokDurumu()
        {
            var cihazlar = await _context.Cihazlar.ToListAsync();
            return View(cihazlar);
        }

        [HttpGet]
        public async Task<IActionResult> MevcutStokDurumuExcel()
        {
            var cihazlar = await _context.Cihazlar.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stok Durumu");
                var currentRow = 1;

                // Baþlýklar
                worksheet.Cell(currentRow, 1).Value = "Cihaz Adý";
                worksheet.Cell(currentRow, 2).Value = "Marka";
                worksheet.Cell(currentRow, 3).Value = "Model";
                worksheet.Cell(currentRow, 4).Value = "Seri No";
                worksheet.Cell(currentRow, 5).Value = "Toplam Adet";
                worksheet.Cell(currentRow, 6).Value = "Mevcut Stok";
                worksheet.Cell(currentRow, 7).Value = "Kullanýmda";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Veriler
                foreach (var cihaz in cihazlar)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = cihaz.CihazAdi;
                    worksheet.Cell(currentRow, 2).Value = cihaz.Marka;
                    worksheet.Cell(currentRow, 3).Value = cihaz.Model;
                    worksheet.Cell(currentRow, 4).Value = cihaz.SeriNumarasi;
                    worksheet.Cell(currentRow, 5).Value = cihaz.ToplamAdet;
                    worksheet.Cell(currentRow, 6).Value = cihaz.MevcutStok;
                    worksheet.Cell(currentRow, 7).Value = cihaz.ToplamAdet - cihaz.MevcutStok;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StokDurumu.xlsx");
                }
            }
        }

        // 2. Teslim Geçmiþi Raporu
        public async Task<IActionResult> TeslimGecmisi(DateTime? baslangicTarihi, DateTime? bitisTarihi, string? islemTipi)
        {
            var query = _context.TeslimIslemleri.Include(t => t.Cihaz).AsQueryable();

            if (baslangicTarihi.HasValue)
            {
                query = query.Where(t => t.TeslimTarihi >= baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                query = query.Where(t => t.TeslimTarihi <= bitisTarihi.Value);
            }

            if (!string.IsNullOrEmpty(islemTipi) && Enum.TryParse<IslemTipi>(islemTipi, out var tip))
            {
                query = query.Where(t => t.IslemTipi == tip);
            }

            ViewData["BaslangicTarihi"] = baslangicTarihi?.ToString("yyyy-MM-dd");
            ViewData["BitisTarihi"] = bitisTarihi?.ToString("yyyy-MM-dd");
            ViewData["IslemTipi"] = islemTipi;

            var islemler = await query.OrderByDescending(t => t.TeslimTarihi).ToListAsync();
            return View(islemler);
        }

        [HttpGet]
        public async Task<IActionResult> TeslimGecmisiExcel(DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var query = _context.TeslimIslemleri.Include(t => t.Cihaz).AsQueryable();

            if (baslangicTarihi.HasValue)
            {
                query = query.Where(t => t.TeslimTarihi >= baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                query = query.Where(t => t.TeslimTarihi <= bitisTarihi.Value);
            }

            var islemler = await query.OrderByDescending(t => t.TeslimTarihi).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Teslim Geçmiþi");
                var currentRow = 1;

                // Baþlýklar
                worksheet.Cell(currentRow, 1).Value = "Ýþlem Tarihi";
                worksheet.Cell(currentRow, 2).Value = "Cihaz";
                worksheet.Cell(currentRow, 3).Value = "Ýþlem Tipi";
                worksheet.Cell(currentRow, 4).Value = "Teslim Eden";
                worksheet.Cell(currentRow, 5).Value = "Teslim Alan";
                worksheet.Cell(currentRow, 6).Value = "Adet";
                worksheet.Cell(currentRow, 7).Value = "Ýade Tarihi";
                worksheet.Cell(currentRow, 8).Value = "Açýklama";

                var headerRange = worksheet.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Veriler
                foreach (var islem in islemler)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = islem.TeslimTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cell(currentRow, 2).Value = islem.Cihaz.CihazAdi;
                    worksheet.Cell(currentRow, 3).Value = islem.IslemTipi.ToString();
                    worksheet.Cell(currentRow, 4).Value = islem.TeslimEden;
                    worksheet.Cell(currentRow, 5).Value = islem.TeslimAlan;
                    worksheet.Cell(currentRow, 6).Value = islem.Adet;
                    worksheet.Cell(currentRow, 7).Value = islem.IadeTarihi?.ToString("dd.MM.yyyy") ?? "-";
                    worksheet.Cell(currentRow, 8).Value = islem.Aciklama ?? "-";
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TeslimGecmisi.xlsx");
                }
            }
        }

        // 3. Kiþiye Göre Zimmetli Cihazlar Raporu
        public async Task<IActionResult> KisiyeGoreZimmetli(string? kisiAdi)
        {
            var query = _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue);

            if (!string.IsNullOrEmpty(kisiAdi))
            {
                query = query.Where(t => t.TeslimAlan.Contains(kisiAdi));
            }

            ViewData["KisiAdi"] = kisiAdi;

            var zimmetliCihazlar = await query
                .OrderBy(t => t.TeslimAlan)
                .ThenBy(t => t.TeslimTarihi)
                .ToListAsync();

            return View(zimmetliCihazlar);
        }

        [HttpGet]
        public async Task<IActionResult> KisiyeGoreZimmetliExcel()
        {
            var zimmetliCihazlar = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue)
                .OrderBy(t => t.TeslimAlan)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Zimmetli Cihazlar");
                var currentRow = 1;

                // Baþlýklar
                worksheet.Cell(currentRow, 1).Value = "Personel Adý";
                worksheet.Cell(currentRow, 2).Value = "Cihaz";
                worksheet.Cell(currentRow, 3).Value = "Marka";
                worksheet.Cell(currentRow, 4).Value = "Model";
                worksheet.Cell(currentRow, 5).Value = "Adet";
                worksheet.Cell(currentRow, 6).Value = "Teslim Tarihi";
                worksheet.Cell(currentRow, 7).Value = "Geçen Süre (Gün)";

                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Veriler
                foreach (var islem in zimmetliCihazlar)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = islem.TeslimAlan;
                    worksheet.Cell(currentRow, 2).Value = islem.Cihaz.CihazAdi;
                    worksheet.Cell(currentRow, 3).Value = islem.Cihaz.Marka;
                    worksheet.Cell(currentRow, 4).Value = islem.Cihaz.Model;
                    worksheet.Cell(currentRow, 5).Value = islem.Adet;
                    worksheet.Cell(currentRow, 6).Value = islem.TeslimTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cell(currentRow, 7).Value = (DateTime.Now - islem.TeslimTarihi).Days;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ZimmetliCihazlar.xlsx");
                }
            }
        }

        // 4. Cihaz Bazlý Hareket Raporu
        public async Task<IActionResult> CihazBazliHareket(int? cihazId)
        {
            ViewData["Cihazlar"] = await _context.Cihazlar.ToListAsync();
            ViewData["CihazId"] = cihazId;

            if (cihazId.HasValue)
            {
                var hareketler = await _context.TeslimIslemleri
                    .Include(t => t.Cihaz)
                    .Where(t => t.CihazId == cihazId.Value)
                    .OrderByDescending(t => t.TeslimTarihi)
                    .ToListAsync();

                return View(hareketler);
            }

            return View(new List<TeslimIslemi>());
        }

        // 5. Marka/Model Bazlý Daðýlým Raporu
        public async Task<IActionResult> MarkaModelDagilim()
        {
            var dagilim = await _context.Cihazlar
                .GroupBy(c => new { c.Marka, c.Model })
                .Select(g => new MarkaModelDagilimViewModel
                {
                    Marka = g.Key.Marka,
                    Model = g.Key.Model,
                    ToplamAdet = g.Sum(c => c.ToplamAdet),
                    MevcutStok = g.Sum(c => c.MevcutStok),
                    KullanimdakiAdet = g.Sum(c => c.ToplamAdet - c.MevcutStok),
                    CihazSayisi = g.Count()
                })
                .OrderBy(d => d.Marka)
                .ThenBy(d => d.Model)
                .ToListAsync();

            return View(dagilim);
        }

        [HttpGet]
        public async Task<IActionResult> MarkaModelDagilimExcel()
        {
            var dagilim = await _context.Cihazlar
                .GroupBy(c => new { c.Marka, c.Model })
                .Select(g => new
                {
                    Marka = g.Key.Marka,
                    Model = g.Key.Model,
                    ToplamAdet = g.Sum(c => c.ToplamAdet),
                    MevcutStok = g.Sum(c => c.MevcutStok),
                    KullanimdakiAdet = g.Sum(c => c.ToplamAdet - c.MevcutStok),
                    CihazSayisi = g.Count()
                })
                .OrderBy(d => d.Marka)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Marka Model Daðýlým");
                var currentRow = 1;

                // Baþlýklar
                worksheet.Cell(currentRow, 1).Value = "Marka";
                worksheet.Cell(currentRow, 2).Value = "Model";
                worksheet.Cell(currentRow, 3).Value = "Cihaz Sayýsý";
                worksheet.Cell(currentRow, 4).Value = "Toplam Adet";
                worksheet.Cell(currentRow, 5).Value = "Mevcut Stok";
                worksheet.Cell(currentRow, 6).Value = "Kullanýmda";

                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Veriler
                foreach (var item in dagilim)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Marka;
                    worksheet.Cell(currentRow, 2).Value = item.Model;
                    worksheet.Cell(currentRow, 3).Value = item.CihazSayisi;
                    worksheet.Cell(currentRow, 4).Value = item.ToplamAdet;
                    worksheet.Cell(currentRow, 5).Value = item.MevcutStok;
                    worksheet.Cell(currentRow, 6).Value = item.KullanimdakiAdet;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MarkaModelDagilim.xlsx");
                }
            }
        }

        // 6. Ýade Edilmemiþ Cihazlar Raporu
        public async Task<IActionResult> IadeEdilmemis()
        {
            var iadeEdilmemis = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue)
                .OrderByDescending(t => t.TeslimTarihi)
                .ToListAsync();

            return View(iadeEdilmemis);
        }

        [HttpGet]
        public async Task<IActionResult> IadeEdilmemisExcel()
        {
            var iadeEdilmemis = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue)
                .OrderByDescending(t => t.TeslimTarihi)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Ýade Edilmemiþ");
                var currentRow = 1;

                // Baþlýklar
                worksheet.Cell(currentRow, 1).Value = "Cihaz";
                worksheet.Cell(currentRow, 2).Value = "Teslim Alan";
                worksheet.Cell(currentRow, 3).Value = "Teslim Tarihi";
                worksheet.Cell(currentRow, 4).Value = "Geçen Süre (Gün)";
                worksheet.Cell(currentRow, 5).Value = "Adet";

                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Veriler
                foreach (var islem in iadeEdilmemis)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = islem.Cihaz.CihazAdi;
                    worksheet.Cell(currentRow, 2).Value = islem.TeslimAlan;
                    worksheet.Cell(currentRow, 3).Value = islem.TeslimTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cell(currentRow, 4).Value = (DateTime.Now - islem.TeslimTarihi).Days;
                    worksheet.Cell(currentRow, 5).Value = islem.Adet;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IadeEdilmemisCihazlar.xlsx");
                }
            }
        }

        // Dashboard Ýstatistikleri
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                ToplamCihazSayisi = await _context.Cihazlar.CountAsync(),
                ToplamStokMiktari = await _context.Cihazlar.SumAsync(c => c.MevcutStok),
                KullanimdakiCihazSayisi = await _context.TeslimIslemleri
                    .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue)
                    .SumAsync(t => t.Adet),
                ToplamIslemSayisi = await _context.TeslimIslemleri.CountAsync(),
                IadeEdilmemisIslemSayisi = await _context.TeslimIslemleri
                    .Where(t => t.IslemTipi == IslemTipi.PersoneleTeslim && !t.IadeTarihi.HasValue)
                    .CountAsync(),
                SonIslemler = await _context.TeslimIslemleri
                    .Include(t => t.Cihaz)
                    .OrderByDescending(t => t.TeslimTarihi)
                    .Take(10)
                    .ToListAsync(),
                DusukStokluCihazlar = await _context.Cihazlar
                    .Where(c => c.MevcutStok < 5)
                    .OrderBy(c => c.MevcutStok)
                    .ToListAsync()
            };

            return View(model);
        }
    }

    // View Model'ler
    public class MarkaModelDagilimViewModel
    {
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int ToplamAdet { get; set; }
        public int MevcutStok { get; set; }
        public int KullanimdakiAdet { get; set; }
        public int CihazSayisi { get; set; }
    }

    public class DashboardViewModel
    {
        public int ToplamCihazSayisi { get; set; }
        public int ToplamStokMiktari { get; set; }
        public int KullanimdakiCihazSayisi { get; set; }
        public int ToplamIslemSayisi { get; set; }
        public int IadeEdilmemisIslemSayisi { get; set; }
        public List<TeslimIslemi> SonIslemler { get; set; } = new List<TeslimIslemi>();
        public List<Cihaz> DusukStokluCihazlar { get; set; } = new List<Cihaz>();
    }
}

