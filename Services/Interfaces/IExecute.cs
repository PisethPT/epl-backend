using System.Data.SqlClient;

namespace PremierLeague_Backend.Services.Interfaces;

public interface IExecute
{
    Task<SqlDataReader> ExecuteReaderAsync(SqlCommand cmd, CancellationToken ct = default);
    Task<T?> ExecuteScalarAsync<T>(SqlCommand cmd, CancellationToken ct = default);
}
