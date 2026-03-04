using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Salario del empleado con vigencia.</summary>
    [Table("salaries")]
    public class Salary
    {
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(1, long.MaxValue, ErrorMessage = "El salario debe ser mayor a 0.")]
        [Display(Name = "Salario")]
        public long SalaryAmount { get; set; }

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