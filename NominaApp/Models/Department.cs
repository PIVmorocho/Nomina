using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Entidad departamento.</summary>
    [Table("departments")]
    public class Department
    {
        [Key]
        [Column("dept_no")]
        public int DeptNo { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Departamento")]
        public string DeptName { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<DeptEmp> DeptEmps { get; set; } = new List<DeptEmp>();
        public ICollection<DeptManager> DeptManagers { get; set; } = new List<DeptManager>();
    }
}