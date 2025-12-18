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
            var selectListItemClubs = new List<SelectListItemClub>();
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
                    selectListItemClubs.Add(item);

                } while (await rdr.ReadAsync(ct));
            }
            return selectListItemClubs;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
