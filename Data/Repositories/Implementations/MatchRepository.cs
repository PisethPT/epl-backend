using System;
using System.Data.SqlClient;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Helper.SqlCommands;
using epl_backend.Models.DTOs;
using epl_backend.Services.Interfaces;
using static epl_backend.Helper.SqlCommands.MatchCommands;

namespace epl_backend.Data.Repositories.Implementations;

public class MatchRepository : IMatchRepository
{
    private readonly IExecute execute;

    public MatchRepository(IExecute execute)
    {
        this.execute = execute;
    }

    public async Task<bool> AddMatchAsync(MatchDto matchDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddMatchCommand;
            cmd.Parameters.AddWithValue("@MatchDate", matchDto.MatchDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@MatchTime", matchDto.MatchTime.ToTimeSpan());
            cmd.Parameters.AddWithValue("@HomeClubId", matchDto.HomeClubId);
            cmd.Parameters.AddWithValue("@AwayClubId", matchDto.AwayClubId);
            cmd.Parameters.AddWithValue("@IsHomeStadium", matchDto.IsHomeStadium);
            cmd.Parameters.AddWithValue("@SeasonId", matchDto.SeasonId);
            cmd.Parameters.AddWithValue("@MatchWeek", matchDto.MatchWeek);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<int> CountMatchesAsync(string? seasonJson = null, string? matchWeekJson = null, string? clubIdJson = null)
    {
        try
        {
            return null!;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<bool> DeleteMatchAsync(int matchId)
    {
        try
        {
            return null!;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> FindClubCannotBeTheSameAsync(int homeClubId, int awayClubId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindClubCannotBeTheSameCommand;
            cmd.Parameters.AddWithValue("@HomeClubId", homeClubId);
            cmd.Parameters.AddWithValue("@AwayClubId", awayClubId);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> FindMatchAlreadyExistsOnTheSelectedDate(DateOnly matchDate, int homeClubId, int awayClubId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchAlreadyExistsOnTheSelectedDateCommand;
            cmd.Parameters.AddWithValue("@MatchDate", matchDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@HomeClubId", homeClubId);
            cmd.Parameters.AddWithValue("@AwayClubId", awayClubId);

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<MatchDto> FindMatchByIdAsync(int matchId)
    {
        try
        {
            return null!;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> FindMatchDateCannotBeEarlierThanToday(DateOnly matchDate)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchDateCannotBeEarlierThanTodayCommand;
            cmd.Parameters.AddWithValue("@MatchDate", matchDate.ToDateTime(TimeOnly.MinValue));

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> FindMatchDateIsOutsideTheActiveSeason(DateOnly matchDate, DateOnly startSeasonDate, DateOnly endSeasonDate)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchDateIsOutsideTheActiveSeasonCommand;
            cmd.Parameters.AddWithValue("@MatchDate", matchDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@StartSeasonDate", startSeasonDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@EndSeasonDate", endSeasonDate.ToDateTime(TimeOnly.MinValue));

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<MatchDetailDto>> GetAllMatchAsync(int? seasonId, int week, int? page = 1, string? seasonJson = null, string? matchWeekJson = null, string? clubIdJson = null, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllMatchesCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@Week", week);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var matches = new List<MatchDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    var match = new MatchDetailDto
                    {
                        MatchId = rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                        MatchWeek = rdr.GetInt32(rdr.GetOrdinal("MatchWeek")),
                        MatchDate = rdr.GetString(rdr.GetOrdinal("MatchDate")),
                        MatchTime = rdr.GetString(rdr.GetOrdinal("MatchTime")),
                        SeasonName = rdr.GetString(rdr.GetOrdinal("SeasonName")),
                        HomeClubId = rdr.GetInt32(rdr.GetOrdinal("HomeClubId")),
                        HomeClubName = rdr.GetString(rdr.GetOrdinal("HomeClubName")),
                        HomeClubCrest = rdr.GetString(rdr.GetOrdinal("HomeClubCrest")),
                        HomeClubTheme = rdr.GetString(rdr.GetOrdinal("HomeClubTheme")),
                        AwayClubId = rdr.GetInt32(rdr.GetOrdinal("AwayClubId")),
                        AwayClubName = rdr.GetString(rdr.GetOrdinal("AwayClubName")),
                        AwayClubCrest = rdr.GetString(rdr.GetOrdinal("AwayClubCrest")),
                        AwayClubTheme = rdr.GetString(rdr.GetOrdinal("AwayClubTheme")),
                        HomeClubScore = rdr.IsDBNull(rdr.GetOrdinal("HomeClubScore")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("HomeClubScore")),
                        AwayClubScore = rdr.IsDBNull(rdr.GetOrdinal("AwayClubScore")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("AwayClubScore")),
                        KickoffStatus = rdr.GetString(rdr.GetOrdinal("KickoffStatus")),
                        IsGameFinish = rdr.GetString(rdr.GetOrdinal("IsGameFinish")),
                        KickoffStadium = rdr.GetString(rdr.GetOrdinal("KickoffStadium"))
                    };
                    matches.Add(match);

                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return matches;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<MatchSeasonDto> GetMatchWeekAsync()
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetMatchWeekCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new MatchSeasonDto
                {
                    SeasonId = rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                    MatchWeek = rdr.IsDBNull(rdr.GetOrdinal("MatchWeek")) ? 1 : rdr.GetInt32(rdr.GetOrdinal("MatchWeek")),
                    StartSeasonDate = rdr.IsDBNull(rdr.GetOrdinal("StartSeasonDate")) ? DateOnly.MinValue : DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("StartSeasonDate"))),
                    EndSeasonDate = rdr.IsDBNull(rdr.GetOrdinal("EndSeasonDate")) ? DateOnly.MinValue : DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("EndSeasonDate")))
                };
            }
            return null;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<MatchWeekDto> GetMatchWeekBySeasonIdAsync(int seasonId, int week, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetMatchWeekBySeasonIdCommand;
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@Week", week);
            var rdr = await execute.ExecuteReaderAsync(cmd);

            if (rdr is not null)
            {
                return new MatchWeekDto
                {
                    Week = rdr.GetInt32(rdr.GetOrdinal("Week")),
                    PrevWeek = rdr.GetInt32(rdr.GetOrdinal("PrevWeek")),
                    NextWeek = rdr.GetInt32(rdr.GetOrdinal("NextWeek")),
                    SeasonId = rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                    MatchWeek = rdr.GetString(rdr.GetOrdinal("MatchWeek")),
                    MatchRangId = rdr.GetString(rdr.GetOrdinal("MatchRangId")),
                    MatchWeekDate = rdr.GetString(rdr.GetOrdinal("MatchWeekDate"))
                };
            }

            return null!;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<bool> UpdateMatchAsync(MatchDto matchDto)
    {
        try
        {
            return null!;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
