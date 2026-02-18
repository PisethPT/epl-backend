using System.Data;
using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using static PremierLeague_Backend.Helper.SqlCommands.ClubCommands;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class ClubRepository : IClubRepository
{
    private readonly string _clubFolder = "upload/clubs";
    public async Task<bool> AddClubAsync(ClubDto club, CancellationToken ct = default)
    {
        if (club == null) throw new ArgumentNullException(nameof(club));

        if (club.CrestFile != null && club.CrestFile.Length > 0)
        {
            var ext = Path.GetExtension(club.CrestFile.FileName ?? string.Empty);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _clubFolder);
            Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            // save file (async)
            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await club.CrestFile.CopyToAsync(stream, ct).ConfigureAwait(false);
            }

            club.Crest = Path.Combine(_clubFolder, safeFileName).Replace('\\', '/');
        }

        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = AddClubCommand;

        cmd.Parameters.AddWithValue("@Name", club.Name ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Founded", club.Founded ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@City", club.City ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Stadium", club.Stadium ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@HeadCoach", club.HeadCoach ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ClubOfficialWebsite", club.ClubOfficialWebsite ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Crest", club.Crest ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Theme", club.Theme ?? (object)DBNull.Value);

        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            if (scalar == null || scalar == DBNull.Value) return false;

            return true;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteClubAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = DeleteClubCommand;
        cmd.Parameters.AddWithValue("@ClubId", id);

        try
        {
            await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            return true;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return false;
        }
    }

    public async Task<bool> ExistsByNameAsync(string clubName, int? clubId, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = ClubExistByNameCommand;

        cmd.Parameters.AddWithValue("@Name", clubName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ClubId", clubId ?? (object)DBNull.Value);
        try
        {
            await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            return false;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return true;
        }
    }

    public async Task<DataTable> GetAllCClubsTableAsync(CancellationToken ct = default)
    {
        var table = new DataTable();
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);

        using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetAllClubsCommand;

        using var adapter = new SqlDataAdapter((SqlCommand)cmd);
        adapter.Fill(table);
        return table;
    }

    public async Task<List<ClubDto>> GetAllClubsAsync(CancellationToken ct = default)
    {
        var list = new List<ClubDto>();
        var table = new DataTable();
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetAllClubsCommand;

        using var adapter = new SqlDataAdapter((SqlCommand)cmd);
        adapter.Fill(table);

        if (table.Rows.Count > 0)
        {
            foreach (DataRow row in table.Rows)
            {
                var club = new ClubDto
                {
                    Id = int.Parse(row["ClubId"].ToString() ?? "0"),
                    Name = row["ClubName"] == DBNull.Value ? string.Empty : Convert.ToString(row["ClubName"]),
                    Founded = row["Founded"] == DBNull.Value ? string.Empty : Convert.ToString(row["Founded"]),
                    City = row["City"] == DBNull.Value ? string.Empty : Convert.ToString(row["City"]),
                    Stadium = row["Stadium"] == DBNull.Value ? string.Empty : Convert.ToString(row["Stadium"]),
                    HeadCoach = row["HeadCoach"] == DBNull.Value ? string.Empty : Convert.ToString(row["HeadCoach"]),
                    ClubOfficialWebsite = row["ClubOfficialWebsite"] == DBNull.Value ? string.Empty : Convert.ToString(row["ClubOfficialWebsite"]),
                    Crest = row["Crest"] == DBNull.Value ? null : Convert.ToString(row["Crest"]),
                    Theme = row["Theme"] == DBNull.Value ? null : Convert.ToString(row["Theme"])
                };
                list.Add(club);
            }
        }

        return list;
    }

    public async Task<DataSet> GetAllClubsDataSetAsync(CancellationToken ct = default)
    {
        var ds = new DataSet();
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);

        using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetAllClubsCommand;

        using var adapter = new SqlDataAdapter((SqlCommand)cmd);
        adapter.Fill(ds);
        return ds;
    }

    public async Task<ClubDto?> GetClubByIdAsync(int id, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetClubByIdCommand;
        cmd.Parameters.AddWithValue("@ClubId", id);

        await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        if (await rdr.ReadAsync(ct).ConfigureAwait(false))
        {
            return new ClubDto
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                Name = rdr.GetString(rdr.GetOrdinal("ClubName")),
                Founded = rdr.GetInt32(rdr.GetOrdinal("Founded")).ToString(),
                City = rdr.GetString(rdr.GetOrdinal("City")),
                Stadium = rdr.GetString(rdr.GetOrdinal("Stadium")),
                HeadCoach = rdr.GetString(rdr.GetOrdinal("HeadCoach")),
                ClubOfficialWebsite = rdr.GetString(rdr.GetOrdinal("ClubOfficialWebsite")),
                Crest = rdr.GetString(rdr.GetOrdinal("Crest")),
                Theme = rdr.GetString(rdr.GetOrdinal("Theme")),
            };
        }

        return null;
    }

    public async Task<DataTable> GetClubByIdTableAsync(int id, CancellationToken ct = default)
    {
        var table = new DataTable();
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);

        using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetClubByIdCommand;
        cmd.Parameters.AddWithValue("@Id", id);

        using var adapter = new SqlDataAdapter((SqlCommand)cmd);
        adapter.Fill(table);
        return table;
    }

    public async Task<bool> UpdateClubAsync(ClubDto club, CancellationToken ct = default)
    {
        if (club == null) throw new ArgumentNullException(nameof(club));

        if (club.CrestFile != null && club.CrestFile.Length > 0)
        {
            var ext = Path.GetExtension(club.CrestFile.FileName ?? string.Empty);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _clubFolder);
            Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            // save file (async)
            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await club.CrestFile.CopyToAsync(stream, ct).ConfigureAwait(false);
            }

            club.Crest = Path.Combine(_clubFolder, safeFileName).Replace('\\', '/');
        }

        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UpdateClubCommand;

        cmd.Parameters.AddWithValue("@ClubId", club.Id);
        cmd.Parameters.AddWithValue("@Name", club.Name ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Founded", club.Founded ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@City", club.City ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Stadium", club.Stadium ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@HeadCoach", club.HeadCoach ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ClubOfficialWebsite", club.ClubOfficialWebsite ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Crest", club.Crest ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Theme", club.Theme ?? (object)DBNull.Value);

        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            if (scalar == null || scalar == DBNull.Value) return false;

            return true;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return false;
        }
    }
}
