using System.Data;
using System.Data.SqlClient;
using epl_backend.Data;
using epl_backend.Services.Interfaces;

namespace epl_backend.Services.Implementations;

public class Execute : IExecute
{
    public async Task<IDataReader> ExecuteReaderAsync(SqlCommand cmd, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        cmd.Connection = conn;
        // cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (await rdr.ReadAsync(ct).ConfigureAwait(false)) return rdr;
            return null!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return null!;
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(SqlCommand cmd, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        cmd.Connection = conn;
        // cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            return scalar == null || scalar == DBNull.Value ? default : (T)Convert.ChangeType(scalar, typeof(T))!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return default;
        }
    }

}
