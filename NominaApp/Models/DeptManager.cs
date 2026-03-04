using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Gerente de departamento con vigencia.</summary>
    [Table("dept_manager")]
    public class DeptManager
    {
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Column("dept_no")]
        public int DeptNo { get; set; }

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
        public Department? Department { get; set; }
    }
}