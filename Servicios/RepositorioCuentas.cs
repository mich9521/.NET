using Dapper;
using Microsoft.Data.SqlClient;
using ManejoPresupuestoV2.Models;

namespace ManejoPresupuestoV2.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int UsuarioId);
        Task Crear(Cuenta cuenta);
      /* Task<Cuenta> Cuenta(int Id, int usuarioId);*/
        Task<Cuenta> ObtenerPorId(int Id, int usuarioId);
    }
    public class RepositorioCuentas:IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear (Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance) 
                                                              VALUES(@Nombre, @TipoCuentaId, @Descripcion, @Balance);

                                                              SELECT  SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;

        }

        public async Task<IEnumerable<Cuenta>> Buscar (int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta from cuentas 
            inner join TiposCuentas tc
            on tc.id = cuentas.TipoCuentaId
            where tc.UsuarioId =@UsuarioId
            order by tc.Orden", new { UsuarioId });

        }

        public async Task<Cuenta> ObtenerPorId(int Id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
            @"select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId from cuentas 
            inner join TiposCuentas tc
            on tc.id = cuentas.TipoCuentaId
            where tc.UsuarioId =@UsuarioId and cuentas.Id =@Id", new {Id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE Cuentas SET 
                                Nombre =@Nombre, Balance =@Balance, Descripcion=@Descripcion,
                                TipoCuentaId =@TipoCuentaId Where Id =@Id", 
                                cuenta);

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("DELETE Cuentas WHERE Id=@Id", new {id});
        }
    }
}
