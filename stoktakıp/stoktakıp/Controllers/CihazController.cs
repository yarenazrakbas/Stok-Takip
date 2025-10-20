using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stoktak�p.Data;
using stoktak�p.Models;

namespace stoktak�p.Controllers
{
    public class CihazController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CihazController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cihaz
        public async Task<IActionResult> Index(string searchString)
        {
            var cihazlar = from c in _context.Cihazlar
                           select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cihazlar = cihazlar.Where(c => c.CihazAdi.Contains(searchString) ||
                                              c.Marka.Contains(searchString) ||
                                              c.Model.Contains(searchString) ||
                                              c.SeriNumarasi.Contains(searchString));
            }

            return View(await cihazlar.OrderByDescending(c => c.KayitTarihi).ToListAsync());
        }

        // GET: Cihaz/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cihaz = await _context.Cihazlar
                .Include(c => c.TeslimIslemleri)
                .FirstOrDefaultAsync(m => m.CihazId == id);

            if (cihaz == null)
            {
                return NotFound();
            }

            return View(cihaz);
        }

        // GET: Cihaz/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cihaz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CihazId,CihazAdi,Marka,Model,SeriNumarasi,ToplamAdet")] Cihaz cihaz)
        {
            // Seri numaras� benzersizlik kontrol�
            if (_context.Cihazlar.Any(c => c.SeriNumarasi == cihaz.SeriNumarasi))
            {
                ModelState.AddModelError("SeriNumarasi", "Bu seri numaras� zaten kay�tl�.");
            }

            if (ModelState.IsValid)
            {
                cihaz.MevcutStok = cihaz.ToplamAdet;
                cihaz.KayitTarihi = DateTime.Now;
                _context.Add(cihaz);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cihaz ba�ar�yla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(cihaz);
        }

        // GET: Cihaz/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cihaz = await _context.Cihazlar.FindAsync(id);
            if (cihaz == null)
            {
                return NotFound();
            }
            return View(cihaz);
        }

        // POST: Cihaz/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CihazId,CihazAdi,Marka,Model,SeriNumarasi,ToplamAdet,MevcutStok,KayitTarihi")] Cihaz cihaz)
        {
            if (id != cihaz.CihazId)
            {
                return NotFound();
            }

            // Seri numaras� benzersizlik kontrol� (kendisi hari�)
            if (_context.Cihazlar.Any(c => c.SeriNumarasi == cihaz.SeriNumarasi && c.CihazId != id))
            {
                ModelState.AddModelError("SeriNumarasi", "Bu seri numaras� zaten kay�tl�.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cihaz);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cihaz ba�ar�yla g�ncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CihazExists(cihaz.CihazId))
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
            return View(cihaz);
        }

        // GET: Cihaz/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cihaz = await _context.Cihazlar
                .FirstOrDefaultAsync(m => m.CihazId == id);
            if (cihaz == null)
            {
                return NotFound();
            }

            return View(cihaz);
        }

        // POST: Cihaz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cihaz = await _context.Cihazlar.FindAsync(id);
            if (cihaz != null)
            {
                // Teslim i�lemi varsa silinmemeli
                var teslimIslemleri = await _context.TeslimIslemleri
                    .Where(t => t.CihazId == id)
                    .CountAsync();

                if (teslimIslemleri > 0)
                {
                    TempData["ErrorMessage"] = "Bu cihaza ait teslim i�lemleri oldu�u i�in silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Cihazlar.Remove(cihaz);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cihaz ba�ar�yla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CihazExists(int id)
        {
            return _context.Cihazlar.Any(e => e.CihazId == id);
        }
    }
}

