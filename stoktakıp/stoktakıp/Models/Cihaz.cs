using System.ComponentModel.DataAnnotations;

namespace stoktak�p.Models
{
    public class Cihaz
    {
        [Key]
        public int CihazId { get; set; }

        [Required(ErrorMessage = "Cihaz ad� zorunludur")]
        [StringLength(100, ErrorMessage = "Cihaz ad� en fazla 100 karakter olabilir")]
        [Display(Name = "Cihaz Ad�")]
        public string CihazAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Marka zorunludur")]
        [StringLength(50, ErrorMessage = "Marka en fazla 50 karakter olabilir")]
        [Display(Name = "Marka")]
        public string Marka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model zorunludur")]
        [StringLength(50, ErrorMessage = "Model en fazla 50 karakter olabilir")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seri numaras� zorunludur")]
        [StringLength(100, ErrorMessage = "Seri numaras� en fazla 100 karakter olabilir")]
        [Display(Name = "Seri Numaras�")]
        public string SeriNumarasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Toplam adet zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Toplam adet en az 1 olmal�d�r")]
        [Display(Name = "Toplam Adet")]
        public int ToplamAdet { get; set; }

        [Required]
        [Display(Name = "Mevcut Stok")]
        public int MevcutStok { get; set; }

        [Display(Name = "Kay�t Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<TeslimIslemi> TeslimIslemleri { get; set; } = new List<TeslimIslemi>();
    }
}

