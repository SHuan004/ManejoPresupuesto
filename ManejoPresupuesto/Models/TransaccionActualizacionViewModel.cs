namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
    {
        public int CuentAnteriorId { get; set; }

        public decimal MontoAnterior { get; set; }

        public string urlRetorno { get; set; }
    }
}
