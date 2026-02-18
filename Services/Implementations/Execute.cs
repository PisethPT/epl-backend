using System.Data;
using System.Data.SqlClient;
using PremierLeague_Backend.Data;
using PremierLeague_Backend.Services.Interfaces;

namespace PremierLeague_Backend.Services.Implementations;

public class Execute : IExecute
{
    public async Task<SqlDataReader?> ExecuteReaderAsync(SqlCommand cmd, CancellationToken ct = default)
    {
        var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        try
        {
            var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, ct).ConfigureAwait(false);
            if (await rdr.ReadAsync(ct).ConfigureAwait(false))
                return rdr;

            rdr.Close();
            return null;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            conn.Dispose();
            return null;
        }
    }


    public async Task<T?> ExecuteScalarAsync<T>(SqlCommand cmd, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            return scalar == null || scalar == DBNull.Value ? default : (T)Convert.ChangeType(scalar, typeof(T))!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            conn.Dispose();
            return default;
        }
    }

}
