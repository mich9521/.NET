using Dapper;
using ManejoPresupuestoV2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoV2.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string Nombre, int UsuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas:IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {

            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(@"TiposCuentas_Insertar", 
                      new {usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre }, commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
           
        }

        public async Task<bool> Existe(string Nombre, int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var existe = await connection.QueryFirstOrDefaultAsync<int>($@"select 1 from TiposCuentas where Nombre = @Nombre and UsuarioId =@UsuarioId;", new {Nombre, UsuarioId});


            return existe == 1;



        }

        public async Task<IEnumerable<TipoCuenta>> Obtener (int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TipoCuenta>("select Id, Nombre, Orden from TiposCuentas where UsuarioId =@UsuarioId ORDER BY Orden", new {usuarioId});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("UPDATE TiposCuentas SET Nombre=@Nombre WHERE Id=@Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id,Nombre,Orden FROM TiposCuentas where Id=@Id AND UsuarioId=@UsuarioId",
                new { id, usuarioId });
        }
        public async  Task Borrar(int id)
        {
            using var connectiom = new SqlConnection(connectionString);

            await connectiom.ExecuteAsync("DELETE TiposCuentas Where Id =@Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden =@Orden Where Id =@Id";

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
      
    }
}
