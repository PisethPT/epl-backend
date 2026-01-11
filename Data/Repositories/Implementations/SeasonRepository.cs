using System.Data;
using System.Data.SqlClient;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.DTOs;
using epl_backend.Services.Interfaces;
using static epl_backend.Helper.SqlCommands.SeasonCommands;

namespace epl_backend.Data.Repositories.Implementations;

public class SeasonRepository : ISeasonRepository
{
    private readonly IExecute execute;

    public SeasonRepository(IExecute execute)
    {
        this.execute = execute;
    }

    public async Task<bool> AddSeasonAsync(SeasonDto seasonDto, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddSeasonCommand;
            cmd.Parameters.AddWithValue("@SeasonName", seasonDto.SeasonName);
            cmd.Parameters.AddWithValue("@StartDate", seasonDto.StartDate!.Value.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@EndDate", seasonDto.EndDate!.Value.ToDateTime(TimeOnly.MinValue));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> CountSeasonsAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = CountSeasonCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            return rdr is not null ? rdr.GetInt32(rdr.GetOrdinal("TotalCount")) : 0;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteSeasonAsync(int seasonId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = DeleteSeasonCommand;
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<SeasonDto> FindSeasonByIdAsync(int seasonId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindSeasonByIdCommand;
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new SeasonDto
                {
                    SeasonId = rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                    SeasonName = rdr.GetString(rdr.GetOrdinal("SeasonName")),
                    StartDate = rdr.IsDBNull(rdr.GetOrdinal("StartDate")) ? DateOnly.MinValue : DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("StartDate"))),
                    EndDate = rdr.IsDBNull(rdr.GetOrdinal("EndDate")) ? DateOnly.MinValue : DateOnly.FromDateTime(rdr.GetDateTime(rdr.GetOrdinal("EndDate")))
                };
            }
            return new SeasonDto();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SeasonDetailDto>> GetAllSeasonAsync(int? page = 1, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllSeasonCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var seasons = new List<SeasonDetailDto>();
            do
            {
                var season = new SeasonDetailDto
                {
                    SeasonId = rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                    SeasonName = rdr.GetString(rdr.GetOrdinal("SeasonName")),
                    StartDate = rdr.GetString(rdr.GetOrdinal("StartDate")),
                    EndDate = rdr.GetString(rdr.GetOrdinal("EndDate"))
                };
                seasons.Add(season);
            } while (await rdr.ReadAsync(ct).ConfigureAwait(false));

            return seasons;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> SeasonExistingAsync(SeasonDto seasonDto, int? seasonId = 0,  CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SeasonExistingCommand;
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@SeasonName", seasonDto.SeasonName);
            cmd.Parameters.AddWithValue("@StartDate", seasonDto.StartDate!.Value.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@EndDate", seasonDto.EndDate!.Value.ToDateTime(TimeOnly.MinValue));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UpdateSeasonAsync(SeasonDto seasonDto, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdateSeasonCommand;
            cmd.Parameters.AddWithValue("@SeasonId", seasonDto.SeasonId);
            cmd.Parameters.AddWithValue("@SeasonName", seasonDto.SeasonName);
            cmd.Parameters.AddWithValue("@StartDate", seasonDto.StartDate!.Value.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@EndDate", seasonDto.EndDate!.Value.ToDateTime(TimeOnly.MinValue));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
