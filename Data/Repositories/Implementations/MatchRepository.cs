using System.Data.SqlClient;
using System.Text.Json;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
using static PremierLeague_Backend.Helper.SqlCommands.MatchCommands;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

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
            cmd.Parameters.Add("@IsHomeStadium", System.Data.SqlDbType.Bit).Value = matchDto.IsHomeStadium;
            cmd.Parameters.AddWithValue("@SeasonId", matchDto.SeasonId);
            cmd.Parameters.AddWithValue("@MatchWeek", matchDto.MatchWeek);
            cmd.Parameters.AddWithValue("@MatchReferees", JsonSerializer.Serialize(matchDto.MatchReferees));

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

    public async Task<bool> DeleteMatchAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = DeleteMatchCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
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

    public async Task<bool> FindMatchAlreadyExistsOnTheSelectedDate(int? matchId, DateOnly matchDate, int homeClubId, int awayClubId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchAlreadyExistsOnTheSelectedDateCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
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

    public async Task<MatchDto> FindMatchByIdAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchByIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new MatchDto
                {
                    MatchId = rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                    MatchDate = DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("MatchDate"))),
                    MatchTime = TimeOnly.FromTimeSpan(rdr.GetTimeSpan(rdr.GetOrdinal("MatchTime"))),
                    HomeClubId = rdr.GetInt32(rdr.GetOrdinal("HomeClubId")),
                    AwayClubId = rdr.GetInt32(rdr.GetOrdinal("AwayClubId")),
                    IsHomeStadium = rdr.GetBoolean(rdr.GetOrdinal("IsHomeStadium")),
                    SeasonId = rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                    MatchWeek = rdr.GetInt32(rdr.GetOrdinal("MatchWeek"))
                };
            }
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

    public async Task<List<MatchRefereeDto>> FindMatchRefereeByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindMatchRefereeByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var matchReferees = new List<MatchRefereeDto>();
            if (rdr is not null)
            {
                do
                {
                    var matchReferee = new MatchRefereeDto
                    {
                        MatchRefereeId = rdr.IsDBNull(rdr.GetOrdinal("MatchRefereeId")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("MatchRefereeId")),
                        MatchId = rdr.IsDBNull(rdr.GetOrdinal("MatchId")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                        RefereeId = rdr.IsDBNull(rdr.GetOrdinal("RefereeId")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("RefereeId")),
                        RoleId = rdr.IsDBNull(rdr.GetOrdinal("RoleId")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("RoleId"))
                    };
                    matchReferees.Add(matchReferee);

                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));

            }
            return matchReferees;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<MatchDetailDto>> GetAllMatchAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllMatchesCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@Week", week);
            cmd.Parameters.AddWithValue("@CompetitionId", competitionId);
            cmd.Parameters.AddWithValue("@ClubIdJson", clubIdJson ?? (object)DBNull.Value);
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

    public async Task<(int matchWeek, int seasonId)> GetCurrentMatchWeekAndSeasonIdAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetCurrentMatchWeekAndSeasonIdCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return (rdr.GetInt32(rdr.GetOrdinal("MatchWeek")), rdr.GetInt32(rdr.GetOrdinal("SeasonId")));
            }
            return (1, 4); // Default match week and season id
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<MatchInfoMatchDetailsDto?> GetMatchInfoMatchDetailsByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetMatchDetailCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new MatchInfoMatchDetailsDto(
                    Kickoff: rdr.IsDBNull(rdr.GetOrdinal("Kickoff")) ? null : rdr.GetString(rdr.GetOrdinal("Kickoff")),
                    Stadium: rdr.IsDBNull(rdr.GetOrdinal("Stadium")) ? null : rdr.GetString(rdr.GetOrdinal("Stadium")),
                    Attendance: rdr.IsDBNull(rdr.GetOrdinal("Attendance")) ? null : rdr.GetString(rdr.GetOrdinal("Attendance"))
                );
            }
            return null;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<MatchInfoMatchOfficialsDto>> GetMatchInfoMatchOfficialsByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = PL_GetMatchOfficialCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var matchOfficials = new List<MatchInfoMatchOfficialsDto>();
            if (rdr is not null)
            {
                do
                {
                    matchOfficials.Add(new MatchInfoMatchOfficialsDto(
                        RefereeRole: rdr.IsDBNull(rdr.GetOrdinal("RefereeRole")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("RefereeRole")),
                        RefereeName: rdr.IsDBNull(rdr.GetOrdinal("RefereeName")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("RefereeName"))
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return matchOfficials;
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

    public async Task<bool> UpdateMatchAsync(MatchDto matchDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdateMatchCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchDto.MatchId);
            cmd.Parameters.AddWithValue("@MatchDate", matchDto.MatchDate.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@MatchTime", matchDto.MatchTime.ToTimeSpan());
            cmd.Parameters.AddWithValue("@HomeClubId", matchDto.HomeClubId);
            cmd.Parameters.AddWithValue("@AwayClubId", matchDto.AwayClubId);
            cmd.Parameters.Add("@IsHomeStadium", System.Data.SqlDbType.Bit).Value = matchDto.IsHomeStadium;
            cmd.Parameters.AddWithValue("@SeasonId", matchDto.SeasonId);
            cmd.Parameters.AddWithValue("@MatchWeek", matchDto.MatchWeek);
            cmd.Parameters.AddWithValue("@MatchReferees", JsonSerializer.Serialize(matchDto.MatchReferees));

            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
