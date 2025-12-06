using System.Data;
using System.Data.SqlClient;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Helper;
using epl_backend.Models.DTOs;
using epl_backend.Models.Enums;
using epl_backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace epl_backend.Data.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IFileStorageService storageService;
    public string _userFolder => "upload/users";
    public UserRepository(IFileStorageService storageService) => this.storageService = storageService;
    public async Task<bool> AddUserAsync(UserDto user, CancellationToken ct = default)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        if (!string.Equals(user.PhotoFile?.FileName, "placeholder.png", StringComparison.OrdinalIgnoreCase))
            user.Photo = await storageService.SavePhotoAsync(user.PhotoFile!, _userFolder);

        var identityUser = new IdentityUser
        {
            UserName = string.Concat(user.FirstName, user.LastName),
            Email = user.Email
        };
        var passwordHasher = new PasswordHasher<IdentityUser>();
        user.PasswordHash = passwordHasher.HashPassword(identityUser, "11");

        //await _userManager.CreateAsync(user);

        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.AddAspNetUserCommand;

        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
        cmd.Parameters.AddWithValue("@LastName", user.LastName);
        cmd.Parameters.AddWithValue("@Gender", user.Gender);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
        cmd.Parameters.AddWithValue("@Photo", user.Photo);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            if (scalar is not null || scalar == DBNull.Value) return false;
            return true;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return false;
        }
    }

    public async Task<bool> CheckPasswordAsync(UserDto userDto, string password, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.CheckAspNetUserPasswordCommand;

        cmd.Parameters.AddWithValue("@UserId", userDto.UserId);

        var storedHashObj = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        if (storedHashObj == null) return false;

        var storedHash = Convert.ToString(storedHashObj);

        var hasher = new PasswordHasher<UserDto>();
        var result = hasher.VerifyHashedPassword(userDto, storedHash!, password);

        return result == PasswordVerificationResult.Success;
    }

    public Task<bool> DeleteUserAsync(int userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.FindAspNetUserByEmailCommand;
        cmd.Parameters.AddWithValue("@Email", email);

        try
        {
            await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (await rdr.ReadAsync(ct).ConfigureAwait(false))
            {
                return new UserDto
                {
                    UserId = rdr.IsDBNull(rdr.GetOrdinal("Id")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("Id")),
                    Email = rdr.IsDBNull(rdr.GetOrdinal("Email")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("Email")),
                    FirstName = rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("FirstName")),
                    LastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("LastName")),
                    Gender = (Gender)rdr.GetInt32(rdr.GetOrdinal("Gender")),
                    PhoneNumber = rdr.IsDBNull(rdr.GetOrdinal("PhoneNumber")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("PhoneNumber"))
                };
            }

            return null!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return null!;
        }
    }

    public async Task<UserDto> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.FindAspNetUserByUserIdCommand;
        cmd.Parameters.AddWithValue("@UserId", userId);

        try
        {
            await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (await rdr.ReadAsync(ct).ConfigureAwait(false))
            {
                return new UserDto
                {
                    UserId = rdr.GetString(rdr.GetOrdinal("Id")),
                    Email = rdr.GetString(rdr.GetOrdinal("Email")),
                    FirstName = rdr.IsDBNull(rdr.GetOrdinal("FirstName")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("FirstName")),
                    LastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("LastName")),
                    PhoneNumber = rdr.IsDBNull(rdr.GetOrdinal("PhoneNumber")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("PhoneNumber")),
                    Gender = (Gender)rdr.GetInt32(rdr.GetOrdinal("Gender")),
                    LockoutEnabled = !rdr.IsDBNull(rdr.GetOrdinal("LockoutEnabled")) && rdr.GetBoolean(rdr.GetOrdinal("LockoutEnabled")),
                    LockoutEnd = rdr.IsDBNull(rdr.GetOrdinal("LockoutEnd")) ? (DateTimeOffset?)null : rdr.GetFieldValue<DateTimeOffset>(rdr.GetOrdinal("LockoutEnd"))
                };
            }

            return null!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return null!;
        }
    }

    public async Task<List<UserDto>> GetAllUsers(CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.GetAllAspNetUsersCommand;
        try
        {
            var users = new List<UserDto>();
            await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            while (await rdr.ReadAsync(ct).ConfigureAwait(false))
            {
                var user = new UserDto
                {
                    UserId = rdr.GetString(rdr.GetOrdinal("Id")),
                    FirstName = rdr.GetString(rdr.GetOrdinal("FirstName")),
                    LastName = rdr.GetString(rdr.GetOrdinal("LastName")),
                    Gender = (Gender)rdr.GetInt32(rdr.GetOrdinal("Gender")),
                    Email = rdr.GetString(rdr.GetOrdinal("Email")),
                    PhoneNumber = rdr.IsDBNull(rdr.GetOrdinal("PhoneNumber")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("PhoneNumber")),
                    Photo = rdr.IsDBNull(rdr.GetOrdinal("Photo")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("Photo"))
                };
                users.Add(user);
            }

            return users;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return null!;
        }
    }

    public async Task<UserRolesDto> GetRolesAsync(UserDto userDto, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = UserCommands.GetAspNetUserRolesCommand;
        cmd.Parameters.AddWithValue("@UserId", userDto.UserId);

        var roles = new List<string>();

        await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await rdr.ReadAsync(ct).ConfigureAwait(false))
        {
            if (!rdr.IsDBNull(0))
                roles.Add(rdr.GetString(0));
        }

        return new UserRolesDto
        {
            UserId = userDto.UserId,
            Roles = roles
        };
    }

    public Task<bool> UpdateUserAsync(UserDto user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
