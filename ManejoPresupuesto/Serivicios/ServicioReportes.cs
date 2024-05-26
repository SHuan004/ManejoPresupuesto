using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Serivicios
{
    public interface IServicioReportes
    {
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtRepTransDet(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtRepTransDetPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag);
    }
    public class ServicioReportes: IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;
        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones,
            IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;

            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioId,int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, año);

            var parametro = new ParaObTransacPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = FechaFin,

            };

            AsignarValoresViewBag(ViewBag,fechaInicio);
            var modelo = await repositorioTransacciones.ObtenerPorSemana(parametro);

            return modelo;
        }


        public async Task<ReporteTransaccionesDetalladas> ObtRepTransDet(int usuarioId,int mes , int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, año);


            var parametro = new ParaObTransacPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = FechaFin,

            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);

            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, FechaFin, transacciones);

            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;

        }

        public async Task<ReporteTransaccionesDetalladas> ObtRepTransDetPorCuenta(int usuarioId, int cuentaId,int mes , int año , dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime FechaFin) = GenerarFechaInicioYFin(mes, año);

            var obtenerTransacciones = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = FechaFin,

            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransacciones);

            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, FechaFin, transacciones);

            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;

        }

        private void AsignarValoresViewBag(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;

            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;

            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;

            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;

            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime FechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetalladas();



            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion).GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });


            modelo.TransaccionesAgrupadas = transaccionesPorFecha;

            modelo.FechaInicio = fechaInicio;

            modelo.FechaFin = FechaFin;
            return modelo;
        }

        private static (DateTime fechaInicio,DateTime FechaFin) GenerarFechaInicioYFin(int mes, int año)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || año <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);

            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }


            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    



    }
}
