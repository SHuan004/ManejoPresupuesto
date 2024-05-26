using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Serivicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParaObTransacPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParaObTransacPorUsuario modelo);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
             
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                                                            new 
                                                            {
                                                                transaccion.UsuarioId,
                                                                transaccion.FechaTransaccion,
                                                                transaccion.Monto,
                                                                transaccion.CategoriaId,
                                                                transaccion.CuentaId,
                                                                transaccion.Nota
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior,int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Actualizar",
                                            new 
                                            {
                                                transaccion.Id,
                                                transaccion.FechaTransaccion,
                                                transaccion.Monto,
                                                transaccion.CategoriaId,
                                                transaccion.CuentaId,
                                                transaccion.Nota,
                                                montoAnterior,
                                                cuentaAnteriorId
                                                
                                            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"Select Transacciones.*, cat.TipoOperacionId
                                                                            From Transacciones 
                                                                            INNER JOIN Categorias cat ON cat.id = Transacciones.CategoriaId
                                                                            WHERE Transacciones.Id = @Id And Transacciones.UsuarioId = @UsuarioId;",
                                                                            new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>(@"Select t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria,
                                                            cu.Nombre as Cuenta, c.TipoOperacionId 
                                                            From Transacciones t
                                                            inner join Categorias c
                                                            on c.Id = t.CategoriaId
                                                            inner join Cuentas cu
                                                            on cu.Id = t.CuentaId
                                                            Where t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId 
                                                            AND FechaTransaccion BETWEEN  @FechaInicio AND @FechaFin;", modelo);

        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParaObTransacPorUsuario modelo)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>(@"Select t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria,
                                                            cu.Nombre as Cuenta, c.TipoOperacionId , t.Nota
                                                            From Transacciones t
                                                            inner join Categorias c
                                                            on c.Id = t.CategoriaId
                                                            inner join Cuentas cu
                                                            on cu.Id = t.CuentaId
                                                            Where t.UsuarioId = @UsuarioId 
                                                            AND FechaTransaccion BETWEEN  @FechaInicio AND @FechaFin
                                                            ORDER BY t.FechaTransaccion DESC ;", modelo);
             
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParaObTransacPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT DATEDIFF(d,@fechaInicio,FechaTransaccion) / 7 +1 as Semana,
                                                                        SUM(Monto) as Monto, cat.TipoOperacionId
                                                                        FROM Transacciones
                                                                        INNER JOIN Categorias cat on cat.Id = Transacciones.CategoriaId
                                                                        WHERE Transacciones.UsuarioId = @UsuarioId AND 
                                                                        FechaTransaccion BETWEEN @fechaInicio and @fechaFin
                                                                        GROUP BY DATEDIFF(d,@fechaInicio,FechaTransaccion) / 7 , cat.TipoOperacionId;",
                                                                        modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId,int año)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto) as Monto, cat.TipoOperacionId 
                                                                        FROM Transacciones 
                                                                        inner join Categorias cat
                                                                        on cat.Id = Transacciones.CategoriaId
                                                                        WHERE Transacciones.UsuarioId = @UsuarioId AND YEAR(FechaTransaccion) = @año
                                                                        GROUP BY Month(FechaTransaccion), cat.TipoOperacionId;", 
                                                                        new { usuarioId, año });}




    }

   
}
