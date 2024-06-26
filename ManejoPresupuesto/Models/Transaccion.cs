﻿using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {

        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "El campo Monto es obligatorio.")]
  
        public decimal Monto { get; set; }

        [Range(1, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoria")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [StringLength(maximumLength: 1000, ErrorMessage ="La nota no sebe pasar de {1} caracteres")]
        public string Nota { get; set; }

        [Display(Name = "Cuenta")]
        [Range(0, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una Cuenta")]
        public int CuentaId { get; set; }

        [Display(Name = "Tipo Operacion")]

        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }

        public string Categoria { get; set; }
    }
}
