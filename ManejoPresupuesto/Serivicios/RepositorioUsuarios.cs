using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Serivicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }
    public class RepositorioUsuarios: IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }    

        public async Task<int> CrearUsuario(Usuario usuario)
        {
         
            using var connection = new SqlConnection(connectionString);
            var Usuarioid = await connection.QuerySingleAsync<int>(@"INSERT INTO Usuarios (Email,EmailNormalizado,PasswordHash)
                                                            values (@Email,@EmailNormalizado,@PasswordHash);

                                                            SELECT SCOPE_IDENTITY();",usuario);

            await connection.ExecuteAsync("CrearDatoUsuarioNuevo", new { Usuarioid }, commandType: System.Data.CommandType.StoredProcedure);

            return Usuarioid;

        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(@"Select * From Usuarios 
                                                                        Where EmailNormalizado = @EmailNormalizado;"
                                                                        ,new {emailNormalizado});
        }   
    }
}
