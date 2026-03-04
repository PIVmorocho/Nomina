using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaApp.Models
{
    /// <summary>Entidad empleado del sistema de nómina.</summary>
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "La CI es obligatoria.")]
        [StringLength(50)]
        [Display(Name = "CI")]
        public string Ci { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [Column(TypeName = "date")]
        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio.")]
        [StringLength(1)]
        [Display(Name = "Género")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de contratación es obligatoria.")]
        [Column(TypeName = "date")]
        [Display(Name = "Fecha Contratación")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [Display(Name = "Correo")]
        public string? Correo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<DeptEmp> DeptEmps { get; set; } = new List<DeptEmp>();
        public ICollection<DeptManager> DeptManagers { get; set; } = new List<DeptManager>();
        public ICollection<Title> Titles { get; set; } = new List<Title>();
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public User? User { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}