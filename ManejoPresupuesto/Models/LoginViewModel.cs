using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "El campo {0} es necesario")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Recuerdame { get; set; }


    }
}
