using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "El maximo de caracteres del campo {1} es de 50")]
        public string Nombre { get; set; }

        [Display(Name = "Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; }

        public int UsuarioId { get; set; }
    }
}
