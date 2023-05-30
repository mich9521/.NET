using Dapper;
using ManejoPresupuestoV2.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoV2.Servicios
{

    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentasId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int anio);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
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
                    transaccion.usuarioId, 
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentasId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                (@"
                select t.Id, t.Monto, t.FechaTransaccion, 
                c.Nombre as Categoria,
                cu.Nombre as Cuenta, c.TipoOperacionID
                from Transacciones t
                INNER JOIN Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE t.CuentaId =@CuentaId
                AND t.usuarioId =@UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin 
                ",modelo);


        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                (@"
                select t.Id, t.Monto, t.FechaTransaccion, 
                c.Nombre as Categoria,
                cu.Nombre as Cuenta, c.TipoOperacionID, Nota
                from Transacciones t
                INNER JOIN Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE 
                t.usuarioId =@UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin 
                ORDER BY t.FechaTransaccion DESC
                ", modelo);


        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
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

            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones.*, cat.TipoOperacionID
                FROM Transacciones
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoriaID
                WHERE Transacciones.Id =@Id AND Transacciones.UsuarioID =@UsuarioId", new {id, usuarioId});
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(
            
            ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
            SELECT datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana,
            sum(Monto) as Monto, cat.TipoOperacionId
            FROM transacciones
            INNER JOIN Categorias cat
            on cat.Id = Transacciones.CategoriaID
            where transacciones.UsuarioID =@usuarioId
            and transacciones.FechaTransaccion BETWEEN @fechaInicio and @fechaFin
            GROUP BY DATEDIFF(d, @fechaInicio, FechaTransaccion)/7, cat.TipoOperacionID

            ", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId,
            
            int anio)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"
            SELECT MONTH(FechaTransaccion) Mes,
            SUM(Monto) Monto,
            cat.TipoOperacionID 
            FROM transacciones
            INNER JOIN categorias cat
            ON cat.Id = transacciones.CategoriaId
            WHERE Transacciones.UsuarioID =@usuarioId
            AND YEAR(FechaTransaccion)=@anio

            GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionID", new {usuarioId, anio});

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure
            );
        }
    }
}


