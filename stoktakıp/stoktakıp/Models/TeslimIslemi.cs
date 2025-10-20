using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stoktak�p.Models
{
    public class TeslimIslemi
    {
        [Key]
        public int IslemId { get; set; }

        [Required(ErrorMessage = "Cihaz se�ilmelidir")]
        [Display(Name = "Cihaz")]
        public int CihazId { get; set; }

        [Required(ErrorMessage = "Teslim eden ki�i ad� zorunludur")]
        [StringLength(100, ErrorMessage = "�sim en fazla 100 karakter olabilir")]
        [Display(Name = "Teslim Eden Ki�i")]
        public string TeslimEden { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teslim alan ki�i ad� zorunludur")]
        [StringLength(100, ErrorMessage = "�sim en fazla 100 karakter olabilir")]
        [Display(Name = "Teslim Alan Ki�i")]
        public string TeslimAlan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teslim tarihi zorunludur")]
        [Display(Name = "Teslim Tarihi")]
        [DataType(DataType.Date)]
        public DateTime TeslimTarihi { get; set; } = DateTime.Now;

        [Display(Name = "�ade Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? IadeTarihi { get; set; }

        [Required(ErrorMessage = "��lem tipi se�ilmelidir")]
        [Display(Name = "��lem Tipi")]
        public IslemTipi IslemTipi { get; set; }

        [Required(ErrorMessage = "Adet zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmal�d�r")]
        [Display(Name = "Adet")]
        public int Adet { get; set; }

        [StringLength(500)]
        [Display(Name = "A��klama")]
        public string? Aciklama { get; set; }

        // Navigation property
        [ForeignKey("CihazId")]
        public virtual Cihaz Cihaz { get; set; } = null!;
    }

    public enum IslemTipi
    {
        [Display(Name = "Stok Giri�i")]
        StokGirisi = 0,

        [Display(Name = "Personele Teslim (��k��)")]
        PersoneleTeslim = 1,

        [Display(Name = "�ade (Giri�)")]
        Iade = 2
    }
}

