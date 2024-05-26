using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Serivicios
{
    public interface IrepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioID,int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int usuarioId, int id);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IrepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {

            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                                    ("TiposCuentas_Insertar", new {usuarioId = tipoCuenta.UsuarioId,
                                                     nombre = tipoCuenta.Nombre},commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;


        }

        public async Task<bool> Existe(string nombre, int usuarioID, int id = 0)
        {

            using var connection = new SqlConnection(connectionString);

            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"SELECT 1 FROM TiposCuentas 
                                                                        WHERE Nombre = @nombre AND UsuarioId = @usuarioID AND Id <> @id;",
                                                                        new { nombre, usuarioID,id });


            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TipoCuenta>(@"Select Id,Nombre,Orden 
                                                            from TiposCuentas
                                                            where UsuarioId = @usuarioId ORDER BY Orden;",
                                                            new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @ID;", 
                                            tipoCuenta);

        }

        public async Task<TipoCuenta> ObtenerPorId(int usuarioId,int id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"Select Id,Nombre,Orden 
                                                                    FROM TiposCuentas  
                                                                    WHERE Id = @Id AND UsuarioId = @UsuarioId;", 
                                                                    new {id,usuarioId});

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id;", new { id });


        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            using var connection = new SqlConnection(connectionString);

            var query = "UPDATE TiposCuentas SET Orden = @Orden WHere Id = @Id;";

            await connection.ExecuteAsync(query,tipoCuentasOrdenados);


        }


    }

}
