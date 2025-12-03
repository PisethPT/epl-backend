using System;
using System.Data;
using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface IClubRepository
{
    // CRUD using DTOs
    Task<bool> AddClubAsync(ClubDto club, CancellationToken ct = default);
    Task<bool> UpdateClubAsync(ClubDto club, CancellationToken ct = default);
    Task<bool> DeleteClubAsync(int id, CancellationToken ct = default);

    Task<ClubDto?> GetClubByIdAsync(int id, CancellationToken ct = default);
    Task<List<ClubDto>> GetAllClubsAsync(CancellationToken ct = default);

    // DataTable / DataSet helpers
    Task<DataTable> GetAllCClubsTableAsync(CancellationToken ct = default);
    Task<DataTable> GetClubByIdTableAsync(int id, CancellationToken ct = default);
    Task<DataSet> GetAllClubsDataSetAsync(CancellationToken ct = default);

    // Validations
    Task<bool> ExistsByNameAsync(string clubName, int? clubId = null, CancellationToken ct = default);
}
