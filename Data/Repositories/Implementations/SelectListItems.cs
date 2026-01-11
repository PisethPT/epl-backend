using System.Data;
using System.Data.SqlClient;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.SelectListItems;
using epl_backend.Services.Interfaces;
using static epl_backend.Helper.SqlCommands.SelectListItemCommands;

namespace epl_backend.Data.Repositories.Implementations;

public class SelectListItems : ISelectListItems
{
    private readonly IExecute execute;

    public SelectListItems(IExecute execute)
    {
        this.execute = execute;
    }
    public async Task<List<SelectListItemClub>> SelectListItemClubAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemClubCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var selectListItemClub = new List<SelectListItemClub>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemClub
                    {
                        ClubId = rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                        ClubName = rdr.IsDBNull(rdr.GetOrdinal("ClubName")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubName")),
                        ClubCrest = rdr.IsDBNull(rdr.GetOrdinal("ClubCrest")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubCrest")),
                        ClubTheme = rdr.IsDBNull(rdr.GetOrdinal("ClubTheme")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubTheme"))
                    };
                    selectListItemClub.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return selectListItemClub;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SelectListItemPlayerLineupByClubId>> SelectListItemPlayerLineupByClubIdAsync(int matchId, int clubId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemPlayerLineupByClubIdCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var SelectListItemPlayer = new List<SelectListItemPlayerLineupByClubId>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemPlayerLineupByClubId
                    (
                        rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                        rdr.IsDBNull(rdr.GetOrdinal("ClubCrest")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubCrest")),
                        rdr.IsDBNull(rdr.GetOrdinal("ClubTheme")) ? "" : rdr.GetString(rdr.GetOrdinal("ClubTheme")),
                        rdr.GetInt32(rdr.GetOrdinal("PlayerId")),
                        rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? "" : rdr.GetString(rdr.GetOrdinal("FirstName")),
                        rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? "" : rdr.GetString(rdr.GetOrdinal("LastName")),
                        rdr.IsDBNull(rdr.GetOrdinal("Photo")) ? "" : rdr.GetString(rdr.GetOrdinal("Photo")),
                        rdr.GetInt32(rdr.GetOrdinal("PlayerNumber")),
                        rdr.IsDBNull(rdr.GetOrdinal("PreferredFoot")) ? "" : rdr.GetString(rdr.GetOrdinal("PreferredFoot")),
                        rdr.GetInt32(rdr.GetOrdinal("PositionId")),
                        rdr.IsDBNull(rdr.GetOrdinal("Position")) ? "" : rdr.GetString(rdr.GetOrdinal("Position"))
                    );
                    SelectListItemPlayer.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return SelectListItemPlayer;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SelectListItemReferee>> SelectListItemRefereeAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemRefereeCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var selectListItem = new List<SelectListItemReferee>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemReferee
                    (
                        rdr.GetInt32(rdr.GetOrdinal("RefereeId")),
                        rdr.IsDBNull(rdr.GetOrdinal("RefereeName")) ? "" : rdr.GetString(rdr.GetOrdinal("RefereeName")),
                        rdr.IsDBNull(rdr.GetOrdinal("DefaultRole")) ? "" : rdr.GetString(rdr.GetOrdinal("DefaultRole")),
                        rdr.IsDBNull(rdr.GetOrdinal("Nationality")) ? "" : rdr.GetString(rdr.GetOrdinal("Nationality"))
                    );
                    selectListItem.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return selectListItem;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SelectListItemRefereeBadgeLevel>> SelectListItemRefereeBadgeLevelAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemRefereeBadgeLevelCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var selectListItem = new List<SelectListItemRefereeBadgeLevel>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemRefereeBadgeLevel
                    (
                        rdr.GetInt32(rdr.GetOrdinal("RefereeBadgeId")),
                        rdr.IsDBNull(rdr.GetOrdinal("BadgeName")) ? "" : rdr.GetString(rdr.GetOrdinal("BadgeName"))
                    );
                    selectListItem.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return selectListItem;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SelectListItemRefereeRole>> SelectListItemRefereeRoleAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemRefereeRoleCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var selectListItem = new List<SelectListItemRefereeRole>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemRefereeRole
                    (
                        rdr.GetInt32(rdr.GetOrdinal("RefereeRoleId")),
                        rdr.IsDBNull(rdr.GetOrdinal("RoleName")) ? "" : rdr.GetString(rdr.GetOrdinal("RoleName"))
                    );
                    selectListItem.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return selectListItem;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<SelectListItemSeason>> SelectListItemSeasonAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = SelectListItemSeasonCommands;
            using var rdr = await execute.ExecuteReaderAsync(cmd);
            var selectListItemSeason = new List<SelectListItemSeason>();
            if (rdr is not null)
            {
                do
                {
                    var item = new SelectListItemSeason(
                        rdr.GetInt32(rdr.GetOrdinal("SeasonId")),
                        rdr.IsDBNull(rdr.GetOrdinal("SeasonName")) ? "" : rdr.GetString(rdr.GetOrdinal("SeasonName")),
                        rdr.IsDBNull(rdr.GetOrdinal("DataSub")) ? "" : rdr.GetString(rdr.GetOrdinal("DataSub"))
                    );
                    selectListItemSeason.Add(item);
                } while (await rdr.ReadAsync(ct));
            }
            return selectListItemSeason;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
