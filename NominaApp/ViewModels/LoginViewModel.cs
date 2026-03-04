using System.ComponentModel.DataAnnotations;

namespace NominaApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public string Clave { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool Recordar { get; set; }
    }
}