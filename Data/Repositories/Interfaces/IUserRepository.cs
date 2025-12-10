using System;
using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDto> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> CheckPasswordAsync(UserDto userDto, string password, CancellationToken ct = default);
    Task<bool> UserByExistEmail(string email, string? userId, CancellationToken ct = default);
    Task<UserRolesDto> GetRolesAsync(UserDto userDto, CancellationToken ct = default);
    Task<UserDto> FindByIdAsync(string userId, CancellationToken ct = default);

    Task<bool> AddUserAsync(UserDto user, CancellationToken ct = default);
    Task<bool> UpdateUserAsync(UserDto user, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(string userId, string? photo, CancellationToken ct = default);
    Task<List<UserDto>> GetAllUsers(CancellationToken ct = default);
}
