using Microsoft.EntityFrameworkCore;
using stoktakýp.Models;

namespace stoktakýp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cihaz> Cihazlar { get; set; }
        public DbSet<TeslimIslemi> TeslimIslemleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seri numarasý benzersiz olmalý
            modelBuilder.Entity<Cihaz>()
                .HasIndex(c => c.SeriNumarasi)
                .IsUnique();

            // Cihaz - TeslimIslemi iliþkisi
            modelBuilder.Entity<TeslimIslemi>()
                .HasOne(t => t.Cihaz)
                .WithMany(c => c.TeslimIslemleri)
                .HasForeignKey(t => t.CihazId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data (örnek veriler)
            modelBuilder.Entity<Cihaz>().HasData(
                new Cihaz
                {
                    CihazId = 1,
                    CihazAdi = "Laptop",
                    Marka = "Dell",
                    Model = "Latitude 5420",
                    SeriNumarasi = "DELL-LAT-001",
                    ToplamAdet = 10,
                    MevcutStok = 10,
                    KayitTarihi = DateTime.Now
                },
                new Cihaz
                {
                    CihazId = 2,
                    CihazAdi = "Mouse",
                    Marka = "Logitech",
                    Model = "M185",
                    SeriNumarasi = "LOG-M185-001",
                    ToplamAdet = 20,
                    MevcutStok = 20,
                    KayitTarihi = DateTime.Now
                },
                new Cihaz
                {
                    CihazId = 3,
                    CihazAdi = "Klavye",
                    Marka = "Logitech",
                    Model = "K120",
                    SeriNumarasi = "LOG-K120-001",
                    ToplamAdet = 15,
                    MevcutStok = 15,
                    KayitTarihi = DateTime.Now
                }
            );
        }
    }
}

