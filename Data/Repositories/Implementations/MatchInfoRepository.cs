using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Helper;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
using static PremierLeague_Backend.Helper.SqlCommands.MatchInfoCommands;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class MatchInfoRepository : IMatchInfoRepository
{
    private readonly IExecute execute;


    public MatchInfoRepository(IExecute execute)
    {
        this.execute = execute;
    }

    public async Task<bool> AddAsync(MatchInfoDto matchInfoDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchInfoDto.MatchId);
            cmd.Parameters.AddWithValue("@Attendance", matchInfoDto.Attendance);
            cmd.Parameters.AddWithValue("@Weather", matchInfoDto.Weather);
            cmd.Parameters.AddWithValue("@PitchCondition", matchInfoDto.PitchCondition);
            cmd.Parameters.AddWithValue("@AddedTimeFirstHalf", matchInfoDto.AddedTimeFirstHalf);
            cmd.Parameters.AddWithValue("@AddedTimeSecondHalf", matchInfoDto.AddedTimeSecondHalf);

            cmd.Parameters.AddWithValue("@MatchReportTitle", matchInfoDto.MatchReportTitle);
            cmd.Parameters.AddWithValue("@MatchReportContent", matchInfoDto.MatchReportContent);
            cmd.Parameters.AddWithValue("@AuthorId", matchInfoDto.AuthorId);
            cmd.Parameters.AddWithValue("@HomeClubReportUrl", matchInfoDto.HomeClubReportUrl);
            cmd.Parameters.AddWithValue("@AwayClubReportUrl", matchInfoDto.AwayClubReportUrl);

            cmd.Parameters.AddWithValue("@PlayerId", matchInfoDto.PlayerId);
            cmd.Parameters.AddWithValue("@AwardBy", matchInfoDto.AwardBy);
            cmd.Parameters.AddWithValue("@Notes", matchInfoDto.Notes);
            cmd.Parameters.AddWithValue("@IsHomeClub", matchInfoDto.IsHomeClub);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database add match info error: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = DeleteCommand;
            cmd.Parameters.AddWithValue("@MatchInfoId", id);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database delete match info error: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<MatchInfoDetailDto>> GetAllAsync(int page = 1, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.CommandText = GetAllCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var values = new List<MatchInfoDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    values.Add(new MatchInfoDetailDto
                    {
                        MatchInfoId = rdr.SafeGetInt("MatchInfoId"),
                        MatchId = rdr.SafeGetInt("MatchId"),
                        Attendance = rdr.SafeGetInt("Attendance"),
                        Weather = rdr.SafeGetString("Weather"),
                        PitchCondition = rdr.SafeGetString("PitchCondition"),
                        AddedTimeFirstHalf = rdr.SafeGetInt("AddedTimeFirstHalf"),
                        AddedTimeSecondHalf = rdr.SafeGetInt("AddedTimeSecondHalf"),
                        MatchDate = rdr.GetString(rdr.GetOrdinal("MatchDate")),
                        MatchTime = rdr.GetString(rdr.GetOrdinal("MatchTime")),
                        SeasonName = rdr.GetString(rdr.GetOrdinal("SeasonName")),
                        HomeClubName = rdr.GetString(rdr.GetOrdinal("HomeClubName")),
                        HomeClubCrest = rdr.GetString(rdr.GetOrdinal("HomeClubCrest")),
                        HomeClubTheme = rdr.GetString(rdr.GetOrdinal("HomeClubTheme")),
                        AwayClubName = rdr.GetString(rdr.GetOrdinal("AwayClubName")),
                        AwayClubCrest = rdr.GetString(rdr.GetOrdinal("AwayClubCrest")),
                        AwayClubTheme = rdr.GetString(rdr.GetOrdinal("AwayClubTheme")),
                        KickoffStadium = rdr.GetString(rdr.GetOrdinal("KickoffStadium"))
                    });
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }

            return values;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database find match info error: {ex.Message}", ex);
        }
    }

    public async Task<MatchInfoDto> FindByIdAsync(int id)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindByIdCommand;
            cmd.Parameters.AddWithValue("@MatchInfoId", id);

            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                var values = new MatchInfoDto
                {
                    MatchInfoId = rdr.SafeGetInt("MatchInfoId"),
                    MatchId = rdr.SafeGetInt("MatchId"),
                    Attendance = rdr.SafeGetInt("Attendance"),
                    Weather = rdr.SafeGetString("Weather"),
                    PitchCondition = rdr.SafeGetString("PitchCondition"),
                    AddedTimeFirstHalf = rdr.SafeGetInt("AddedTimeFirstHalf"),
                    AddedTimeSecondHalf = rdr.SafeGetInt("AddedTimeSecondHalf"),

                    MatchReportTitle = rdr.SafeGetString("MatchReportTitle"),
                    MatchReportContent = rdr.SafeGetString("MatchReportContent"),
                    HomeClubReportUrl = rdr.SafeGetString("HomeClubReportUrl"),
                    AwayClubReportUrl = rdr.SafeGetString("AwayClubReportUrl"),

                    PlayerId = rdr.SafeGetInt("PlayerId"),
                    AwardBy = rdr.SafeGetString("AwardedBy"),
                    Notes = rdr.SafeGetString("Notes"),
                    IsHomeClub = rdr.SafeGetBoolean("IsHomeClub"),
                };

                return values;
            }

            return new MatchInfoDto();
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database find match info error: {ex.Message}", ex);
        }
    }

    public Task<bool> IsExistAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(MatchInfoDto matchInfoDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdateCommand;
            cmd.Parameters.AddWithValue("@MatchInfoId", matchInfoDto.MatchInfoId);
            cmd.Parameters.AddWithValue("@MatchId", matchInfoDto.MatchId);
            cmd.Parameters.AddWithValue("@Attendance", matchInfoDto.Attendance);
            cmd.Parameters.AddWithValue("@Weather", matchInfoDto.Weather);
            cmd.Parameters.AddWithValue("@PitchCondition", matchInfoDto.PitchCondition);
            cmd.Parameters.AddWithValue("@AddedTimeFirstHalf", matchInfoDto.AddedTimeFirstHalf);
            cmd.Parameters.AddWithValue("@AddedTimeSecondHalf", matchInfoDto.AddedTimeSecondHalf);

            cmd.Parameters.AddWithValue("@MatchReportTitle", matchInfoDto.MatchReportTitle);
            cmd.Parameters.AddWithValue("@MatchReportContent", matchInfoDto.MatchReportContent);
            cmd.Parameters.AddWithValue("@AuthorId", matchInfoDto.AuthorId);
            cmd.Parameters.AddWithValue("@HomeClubReportUrl", matchInfoDto.HomeClubReportUrl);
            cmd.Parameters.AddWithValue("@AwayClubReportUrl", matchInfoDto.AwayClubReportUrl);

            cmd.Parameters.AddWithValue("@PlayerId", matchInfoDto.PlayerId);
            cmd.Parameters.AddWithValue("@AwardBy", matchInfoDto.AwardBy);
            cmd.Parameters.AddWithValue("@Notes", matchInfoDto.Notes);
            cmd.Parameters.AddWithValue("@IsHomeClub", matchInfoDto.IsHomeClub);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database update match info error: {ex.Message}", ex);
        }
    }

    public async Task<int> GetCountAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = CountCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            return rdr is not null ? rdr.GetInt32(rdr.GetOrdinal("TotalCount")) : 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database count match info error: {ex.Message}", ex);
        }
    }
}
