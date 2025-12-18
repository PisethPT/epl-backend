using System.Data;
using System.Data.SqlClient;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.DTOs;
using epl_backend.Models.Enums;
using epl_backend.Services.Interfaces;
using static epl_backend.Helper.SqlCommands.PlayerCommands;

namespace epl_backend.Data.Repositories.Implementations;

public class PlayerRepository : IPlayerRepository
{
    private readonly IExecute execute;
    private readonly IFileStorageService storageService;
    private string _playerFolder => "upload/players";

    public PlayerRepository(IExecute execute, IFileStorageService storageService)
    {
        this.execute = execute;
        this.storageService = storageService;
    }

    public async Task<bool> AddPlayerAsync(PlayerDto playerDto, CancellationToken ct = default)
    {
        if (playerDto is null) throw new ArgumentNullException(nameof(playerDto));
        if (!string.Equals(playerDto.PhotoFile?.FileName, "placeholder.png", StringComparison.OrdinalIgnoreCase))
            playerDto.Photo = await storageService.SavePhotoAsync(playerDto.PhotoFile!, _playerFolder);
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddPlayerCommand;

            cmd.Parameters.AddWithValue("@FirstName", playerDto.FirstName);
            cmd.Parameters.AddWithValue("@LastName", playerDto.LastName);
            cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = playerDto.DateOfBirth!.Value.ToDateTime(TimeOnly.MinValue) });
            cmd.Parameters.AddWithValue("@PlaceOfBirth", playerDto.PlaceOfBirth);
            cmd.Parameters.AddWithValue("@Nationality", playerDto.Nationality);
            cmd.Parameters.AddWithValue("@PreferredFoot", playerDto.PreferredFoot);
            cmd.Parameters.AddWithValue("@Height", playerDto.Height);
            cmd.Parameters.AddWithValue("@Position", playerDto.Position);
            cmd.Parameters.AddWithValue("@PlayerNumber", playerDto.PlayerNumber);
            cmd.Parameters.AddWithValue("@JoinedClub", playerDto.JoinedClub);
            cmd.Parameters.AddWithValue("@Photo", playerDto.Photo);
            cmd.Parameters.AddWithValue("@ClubId", playerDto.ClubId);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;

        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeletePlayerAsync(int playerId, string? photo, CancellationToken ct = default)
    {
        var cmd = new SqlCommand();
        cmd.CommandText = DeletePlayerCommand;
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        try
        {
            var scalar = await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
            if (scalar)
            {
                if (!string.IsNullOrEmpty(photo) && !string.IsNullOrWhiteSpace(photo))
                {
                    await storageService.DeleteFileAsync(Path.Combine(_playerFolder, photo));
                }
            }

            return scalar;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> PlayerExistingByClubAsync(PlayerDto playerDto, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = PlayerExistingByClubCommand;
            cmd.Parameters.AddWithValue("@PlayerId", playerDto.PlayerId);
            cmd.Parameters.AddWithValue("@ClubId", playerDto.ClubId);
            cmd.Parameters.AddWithValue("@FirstName", playerDto.FirstName);
            cmd.Parameters.AddWithValue("@LastName", playerDto.LastName);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UpdatePlayerAsync(PlayerDto playerDto, CancellationToken ct = default)
    {
        if (playerDto is null) throw new ArgumentNullException(nameof(playerDto));
        if (playerDto.PhotoFile is not null || playerDto.PhotoFile?.Length > 0)
        {
            if (!string.Equals(playerDto.PhotoFile?.FileName, "placeholder.png", StringComparison.OrdinalIgnoreCase))
                playerDto.Photo = await storageService.SavePhotoAsync(playerDto.PhotoFile!, _playerFolder);
        }

        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdatePlayerCommand;

            cmd.Parameters.AddWithValue("@PlayerId", playerDto.PlayerId);
            cmd.Parameters.AddWithValue("@FirstName", playerDto.FirstName);
            cmd.Parameters.AddWithValue("@LastName", playerDto.LastName);
            cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = playerDto.DateOfBirth!.Value.ToDateTime(TimeOnly.MinValue) });
            cmd.Parameters.AddWithValue("@PlaceOfBirth", playerDto.PlaceOfBirth);
            cmd.Parameters.AddWithValue("@Nationality", playerDto.Nationality);
            cmd.Parameters.AddWithValue("@PreferredFoot", playerDto.PreferredFoot);
            cmd.Parameters.AddWithValue("@Height", playerDto.Height);
            cmd.Parameters.AddWithValue("@Position", playerDto.Position);
            cmd.Parameters.AddWithValue("@PlayerNumber", playerDto.PlayerNumber);
            cmd.Parameters.AddWithValue("@JoinedClub", playerDto.JoinedClub);
            cmd.Parameters.AddWithValue("@Photo", playerDto.Photo);
            cmd.Parameters.AddWithValue("@ClubId", playerDto.ClubId);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;

        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<PlayerDetailDto>> GetAllPlayerAsync(string? positionJson = null , string? clubIdJson = null, int? page = 1, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllPlayerCommand;

            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", 20);
            cmd.Parameters.AddWithValue("@PositionJson", positionJson);
            cmd.Parameters.AddWithValue("@ClubIdJson", clubIdJson);

            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var playerDetailDtos = new List<PlayerDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    var item = new PlayerDetailDto
                    {
                        PlayerId = rdr.GetInt32(rdr.GetOrdinal("PlayerId")),
                        FirstName = rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName")),
                        LastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")),
                        DateOfBirth = rdr.IsDBNull(rdr.GetOrdinal("DateOfBirth")) ? "" : rdr.GetString(rdr.GetOrdinal("DateOfBirth")),
                        PlaceOfBirth = rdr.IsDBNull(rdr.GetOrdinal("PlaceOfBirth")) ? "" : rdr.GetString(rdr.GetOrdinal("PlaceOfBirth")),
                        Nationality = rdr.IsDBNull(rdr.GetOrdinal("Nationality")) ? "" : rdr.GetString(rdr.GetOrdinal("Nationality")),
                        PreferredFoot = MapPreferredFoot(rdr["PreferredFoot"]),
                        Height = rdr.IsDBNull(rdr.GetOrdinal("Height")) ? "" : rdr.GetString(rdr.GetOrdinal("Height")),
                        Position = MapPosition(rdr["Position"]),
                        PlayerNumber = rdr.GetInt32(rdr.GetOrdinal("PlayerNumber")),
                        JoinedClub = rdr.IsDBNull(rdr.GetOrdinal("JoinedClub")) ? "" : rdr.GetString(rdr.GetOrdinal("JoinedClub")),
                        Photo = rdr.IsDBNull(rdr.GetOrdinal("Photo")) ? "" : rdr.GetString(rdr.GetOrdinal("Photo")),
                        ClubId = rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                        ClubName = rdr.IsDBNull(rdr.GetOrdinal("ClubName")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubName")),
                        ClubCrest = rdr.IsDBNull(rdr.GetOrdinal("ClubCrest")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubCrest")),
                        ClubTheme = rdr.IsDBNull(rdr.GetOrdinal("ClubTheme")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubTheme"))
                    };
                    playerDetailDtos.Add(item);

                } while (await rdr.ReadAsync(ct));
            }

            return playerDetailDtos;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PlayerDto> FindPlayerByIdAsync(int playerId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindPlayerByIdCommand;

            cmd.Parameters.AddWithValue("@PlayerId", playerId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new PlayerDto
                {
                    PlayerId = rdr.GetInt32(rdr.GetOrdinal("PlayerId")),
                    FirstName = rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName")),
                    LastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")),
                    DateOfBirth = rdr.IsDBNull(rdr.GetOrdinal("DateOfBirth")) ? DateOnly.MinValue : DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("DateOfBirth"))),
                    PlaceOfBirth = rdr.IsDBNull(rdr.GetOrdinal("PlaceOfBirth")) ? "" : rdr.GetString(rdr.GetOrdinal("PlaceOfBirth")),
                    Nationality = rdr.IsDBNull(rdr.GetOrdinal("Nationality")) ? "" : rdr.GetString(rdr.GetOrdinal("Nationality")),
                    PreferredFoot = MapPreferredFoot(rdr["PreferredFoot"]),
                    Height = rdr.IsDBNull(rdr.GetOrdinal("Height")) ? "" : rdr.GetString(rdr.GetOrdinal("Height")),
                    Position = MapPosition(rdr["Position"]),
                    PlayerNumber = rdr.GetInt32(rdr.GetOrdinal("PlayerNumber")),
                    JoinedClub = rdr.IsDBNull(rdr.GetOrdinal("JoinedClub")) ? "" : rdr.GetString(rdr.GetOrdinal("JoinedClub")),
                    Photo = rdr.IsDBNull(rdr.GetOrdinal("Photo")) ? "" : rdr.GetString(rdr.GetOrdinal("Photo")),
                    ClubId = rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                };
            }

            return new PlayerDto();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> CountPlayersAsync(string? positionJson = null , string? clubIdJson = null, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = CountPlayersClubCommand;
            cmd.Parameters.AddWithValue("@PositionJson", positionJson);
            cmd.Parameters.AddWithValue("@ClubIdJson", clubIdJson);

            var rdr = await execute.ExecuteReaderAsync(cmd);
            return rdr is not null ? rdr.GetInt32(rdr.GetOrdinal("TotalCount")) : 0;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    Position MapPosition(object dbValue)
    {
        if (dbValue == null)
            return Position.Goalkeeper;

        if (dbValue is int intVal)
        {
            return Enum.IsDefined(typeof(Position), intVal)
                ? (Position)intVal
                : Position.Goalkeeper;
        }

        if (dbValue is string strVal)
        {
            strVal = strVal.Trim();

            if (int.TryParse(strVal, out int parsedInt))
            {
                return Enum.IsDefined(typeof(Position), parsedInt)
                    ? (Position)parsedInt
                    : Position.Goalkeeper;
            }

            if (Enum.TryParse<Position>(strVal, true, out var position))
            {
                return position;
            }
        }

        return Position.Goalkeeper;
    }

    PreferredFoot MapPreferredFoot(object dbValue)
    {
        if (dbValue == null)
            return PreferredFoot.Right;

        if (dbValue is int intVal)
        {
            return Enum.IsDefined(typeof(PreferredFoot), intVal)
                ? (PreferredFoot)intVal
                : PreferredFoot.Right;
        }

        if (dbValue is string strVal)
        {
            strVal = strVal.Trim();

            if (int.TryParse(strVal, out int parsedInt))
            {
                return Enum.IsDefined(typeof(PreferredFoot), parsedInt)
                    ? (PreferredFoot)parsedInt
                    : PreferredFoot.Right;
            }

            if (Enum.TryParse<PreferredFoot>(strVal, true, out var foot))
            {
                return foot;
            }
        }

        return PreferredFoot.Right;
    }

}
