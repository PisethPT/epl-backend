using System;
using System.Data.SqlClient;

namespace epl_backend.Helper;

public static class DataReader
{
    public static string SafeGetString(SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? string.Empty : Convert.ToString(r.GetValue(idx));
    }

    public static string? SafeGetStringOrNull(SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? null : Convert.ToString(r.GetValue(idx));
    }

    public static int SafeGetInt(SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? 0 : r.GetInt32(idx);
    }
}
