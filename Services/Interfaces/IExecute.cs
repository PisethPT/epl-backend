using System.Data;
using System.Data.SqlClient;

namespace epl_backend.Services.Interfaces;

public interface IExecute
{
    Task<IDataReader> ExecuteReaderAsync(SqlCommand cmd, CancellationToken ct = default);
    Task<T?> ExecuteScalarAsync<T>(SqlCommand cmd, CancellationToken ct = default);
}
