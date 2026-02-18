using System.Data.SqlClient;

namespace PremierLeague_Backend.Helper;

public static class DataReader
{
    public static string SafeGetString(this SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? string.Empty : Convert.ToString(r.GetValue(idx))!;
    }

    public static string? SafeGetStringOrNull(this SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? null : Convert.ToString(r.GetValue(idx));
    }

    public static int SafeGetInt(this SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? 0 : r.GetInt32(idx);
    }

    public static bool SafeGetBoolean(this SqlDataReader r, string colName)
    {
        int idx = r.GetOrdinal(colName);
        return r.IsDBNull(idx) ? false : r.GetBoolean(idx);
    }
}
