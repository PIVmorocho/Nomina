using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Log de auditoría de cambios salariales.</summary>
    [Table("Log_AuditoriaSalarios")]
    public class LogAuditoriaSalarios
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        [Display(Name = "Fecha")]
        public DateTime FechaActualizacion { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Detalle")]
        public string DetalleCambio { get; set; } = string.Empty;

        [Display(Name = "Salario")]
        public long Salario { get; set; }

        [Column("emp_no")]
        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        // Navegación
        public Employee? Employee { get; set; }
    }
}