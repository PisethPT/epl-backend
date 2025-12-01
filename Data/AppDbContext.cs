
using System.Data.SqlClient;

namespace epl_backend.Data
{
    public sealed class AppDbContext : IDisposable
    {
        private static readonly object _initLock = new();
        private static AppDbContext? _instance;
        private readonly string _connectionString;
        private bool _disposed;

        private AppDbContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public static void Initialize(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            lock (_initLock)
            {
                if (_instance != null) return;

                var conn = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                _instance = new AppDbContext(conn);
            }
        }

        public static AppDbContext Instance
        {
            get
            {
                if (_instance == null) throw new InvalidOperationException("AppDbContext not initialized. Call AppDbContext.Initialize(configuration) at startup.");
                return _instance;
            }
        }

        // Returns an open connection; caller disposes.
        public async Task<SqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
