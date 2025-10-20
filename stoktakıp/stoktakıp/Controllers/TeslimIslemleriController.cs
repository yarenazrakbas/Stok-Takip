using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using stoktakýp.Data;
using stoktakýp.Models;

namespace stoktakýp.Controllers
{
    public class TeslimIslemleriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeslimIslemleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TeslimIslemleri
        public async Task<IActionResult> Index(string searchString, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var islemler = from t in _context.TeslimIslemleri.Include(t => t.Cihaz)
                           select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                islemler = islemler.Where(t => t.TeslimEden.Contains(searchString) ||
                                              t.TeslimAlan.Contains(searchString) ||
                                              t.Cihaz.CihazAdi.Contains(searchString));
            }

            if (baslangicTarihi.HasValue)
            {
                islemler = islemler.Where(t => t.TeslimTarihi >= baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                islemler = islemler.Where(t => t.TeslimTarihi <= bitisTarihi.Value);
            }

            ViewData["SearchString"] = searchString;
            ViewData["BaslangicTarihi"] = baslangicTarihi?.ToString("yyyy-MM-dd");
            ViewData["BitisTarihi"] = bitisTarihi?.ToString("yyyy-MM-dd");

            return View(await islemler.OrderByDescending(t => t.TeslimTarihi).ToListAsync());
        }

        // GET: TeslimIslemleri/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teslimIslemi = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .FirstOrDefaultAsync(m => m.IslemId == id);

            if (teslimIslemi == null)
            {
                return NotFound();
            }

            return View(teslimIslemi);
        }

        // GET: TeslimIslemleri/Create
        public IActionResult Create()
        {
            ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi");
            return View();
        }

        // POST: TeslimIslemleri/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IslemId,CihazId,TeslimEden,TeslimAlan,TeslimTarihi,IadeTarihi,IslemTipi,Adet,Aciklama")] TeslimIslemi teslimIslemi)
        {
            if (ModelState.IsValid)
            {
                var cihaz = await _context.Cihazlar.FindAsync(teslimIslemi.CihazId);

                if (cihaz == null)
                {
                    ModelState.AddModelError("CihazId", "Seçilen cihaz bulunamadý.");
                    ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi", teslimIslemi.CihazId);
                    return View(teslimIslemi);
                }

                // Ýþlem tipine göre stok kontrolü
                if (teslimIslemi.IslemTipi == IslemTipi.PersoneleTeslim)
                {
                    // Personele teslim - stok azalýr
                    if (cihaz.MevcutStok < teslimIslemi.Adet)
                    {
                        ModelState.AddModelError("Adet", $"Yetersiz stok! Mevcut stok: {cihaz.MevcutStok}");
                        ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi", teslimIslemi.CihazId);
                        return View(teslimIslemi);
                    }
                    cihaz.MevcutStok -= teslimIslemi.Adet;
                }
                else if (teslimIslemi.IslemTipi == IslemTipi.StokGirisi)
                {
                    // Stok giriþi - stok artar
                    cihaz.MevcutStok += teslimIslemi.Adet;
                    cihaz.ToplamAdet += teslimIslemi.Adet;
                }
                else if (teslimIslemi.IslemTipi == IslemTipi.Iade)
                {
                    // Ýade - stok artar
                    cihaz.MevcutStok += teslimIslemi.Adet;
                    teslimIslemi.IadeTarihi = DateTime.Now;
                }

                _context.Add(teslimIslemi);
                _context.Update(cihaz);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Teslim iþlemi baþarýyla kaydedildi.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi", teslimIslemi.CihazId);
            return View(teslimIslemi);
        }

        // GET: TeslimIslemleri/IadeEt/5
        public async Task<IActionResult> IadeEt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teslimIslemi = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .FirstOrDefaultAsync(m => m.IslemId == id);

            if (teslimIslemi == null)
            {
                return NotFound();
            }

            if (teslimIslemi.IadeTarihi.HasValue)
            {
                TempData["ErrorMessage"] = "Bu cihaz zaten iade edilmiþ.";
                return RedirectToAction(nameof(Index));
            }

            return View(teslimIslemi);
        }

        // POST: TeslimIslemleri/IadeEt/5
        [HttpPost, ActionName("IadeEt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IadeEtConfirmed(int id)
        {
            var teslimIslemi = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .FirstOrDefaultAsync(m => m.IslemId == id);

            if (teslimIslemi == null)
            {
                return NotFound();
            }

            if (teslimIslemi.IadeTarihi.HasValue)
            {
                TempData["ErrorMessage"] = "Bu cihaz zaten iade edilmiþ.";
                return RedirectToAction(nameof(Index));
            }

            // Ýade iþlemi
            teslimIslemi.IadeTarihi = DateTime.Now;
            teslimIslemi.Cihaz.MevcutStok += teslimIslemi.Adet;

            _context.Update(teslimIslemi);
            _context.Update(teslimIslemi.Cihaz);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cihaz baþarýyla iade edildi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: TeslimIslemleri/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teslimIslemi = await _context.TeslimIslemleri.FindAsync(id);
            if (teslimIslemi == null)
            {
                return NotFound();
            }
            ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi", teslimIslemi.CihazId);
            return View(teslimIslemi);
        }

        // POST: TeslimIslemleri/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IslemId,CihazId,TeslimEden,TeslimAlan,TeslimTarihi,IadeTarihi,IslemTipi,Adet,Aciklama")] TeslimIslemi teslimIslemi)
        {
            if (id != teslimIslemi.IslemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teslimIslemi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Teslim iþlemi baþarýyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeslimIslemiExists(teslimIslemi.IslemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CihazId"] = new SelectList(_context.Cihazlar, "CihazId", "CihazAdi", teslimIslemi.CihazId);
            return View(teslimIslemi);
        }

        // GET: TeslimIslemleri/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teslimIslemi = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .FirstOrDefaultAsync(m => m.IslemId == id);
            if (teslimIslemi == null)
            {
                return NotFound();
            }

            return View(teslimIslemi);
        }

        // POST: TeslimIslemleri/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teslimIslemi = await _context.TeslimIslemleri
                .Include(t => t.Cihaz)
                .FirstOrDefaultAsync(m => m.IslemId == id);

            if (teslimIslemi != null)
            {
                // Stok düzeltmesi
                if (teslimIslemi.IslemTipi == IslemTipi.PersoneleTeslim && !teslimIslemi.IadeTarihi.HasValue)
                {
                    teslimIslemi.Cihaz.MevcutStok += teslimIslemi.Adet;
                }
                else if (teslimIslemi.IslemTipi == IslemTipi.StokGirisi)
                {
                    teslimIslemi.Cihaz.MevcutStok -= teslimIslemi.Adet;
                    teslimIslemi.Cihaz.ToplamAdet -= teslimIslemi.Adet;
                }
                else if (teslimIslemi.IslemTipi == IslemTipi.Iade)
                {
                    teslimIslemi.Cihaz.MevcutStok -= teslimIslemi.Adet;
                }

                _context.Update(teslimIslemi.Cihaz);
                _context.TeslimIslemleri.Remove(teslimIslemi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Teslim iþlemi baþarýyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TeslimIslemiExists(int id)
        {
            return _context.TeslimIslemleri.Any(e => e.IslemId == id);
        }
    }
}

