using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Título/cargo histórico del empleado.</summary>
    [Table("titles")]
    public class Title
    {
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Título")]
        public string TitleName { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        [Display(Name = "Desde")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Hasta")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        // Navegación
        public Employee? Employee { get; set; }
    }
}