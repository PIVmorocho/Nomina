using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Usuario de la aplicación.</summary>
    [Table("users")]
    public class User
    {
        [Key]
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Column("emp_no")]
        [Display(Name = "Empleado")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(100)]
        [Display(Name = "Clave")]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "RRHH";

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public Employee? Employee { get; set; }
    }
}