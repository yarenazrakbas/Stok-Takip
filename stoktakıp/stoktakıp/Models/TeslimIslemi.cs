using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stoktakýp.Models
{
    public class TeslimIslemi
    {
        [Key]
        public int IslemId { get; set; }

        [Required(ErrorMessage = "Cihaz seçilmelidir")]
        [Display(Name = "Cihaz")]
        public int CihazId { get; set; }

        [Required(ErrorMessage = "Teslim eden kiþi adý zorunludur")]
        [StringLength(100, ErrorMessage = "Ýsim en fazla 100 karakter olabilir")]
        [Display(Name = "Teslim Eden Kiþi")]
        public string TeslimEden { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teslim alan kiþi adý zorunludur")]
        [StringLength(100, ErrorMessage = "Ýsim en fazla 100 karakter olabilir")]
        [Display(Name = "Teslim Alan Kiþi")]
        public string TeslimAlan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teslim tarihi zorunludur")]
        [Display(Name = "Teslim Tarihi")]
        [DataType(DataType.Date)]
        public DateTime TeslimTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Ýade Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? IadeTarihi { get; set; }

        [Required(ErrorMessage = "Ýþlem tipi seçilmelidir")]
        [Display(Name = "Ýþlem Tipi")]
        public IslemTipi IslemTipi { get; set; }

        [Required(ErrorMessage = "Adet zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalýdýr")]
        [Display(Name = "Adet")]
        public int Adet { get; set; }

        [StringLength(500)]
        [Display(Name = "Açýklama")]
        public string? Aciklama { get; set; }

        // Navigation property
        [ForeignKey("CihazId")]
        public virtual Cihaz Cihaz { get; set; } = null!;
    }

    public enum IslemTipi
    {
        [Display(Name = "Stok Giriþi")]
        StokGirisi = 0,

        [Display(Name = "Personele Teslim (Çýkýþ)")]
        PersoneleTeslim = 1,

        [Display(Name = "Ýade (Giriþ)")]
        Iade = 2
    }
}

