using System.Data;
using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.Enums;
using PremierLeague_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using static PremierLeague_Backend.Helper.SqlCommands.UserCommands;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IFileStorageService storageService;
    private readonly IExecute execute;
    public string _userFolder => "upload/users";
    public UserRepository(IFileStorageService storageService, IExecute execute)
    {
        this.storageService = storageService;
        this.execute = execute;
    }

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

        var cmd = new SqlCommand();
        cmd.CommandText = AddAspNetUserCommand;

        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
        cmd.Parameters.AddWithValue("@LastName", user.LastName);
        cmd.Parameters.AddWithValue("@Gender", user.Gender);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
        cmd.Parameters.AddWithValue("@Photo", user.Photo);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        try
        {
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> CheckPasswordAsync(UserDto userDto, string password, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = CheckAspNetUserPasswordCommand;

        cmd.Parameters.AddWithValue("@UserId", userDto.UserId);

        var storedHashObj = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        if (storedHashObj == null) return false;

        var storedHash = Convert.ToString(storedHashObj);

        var hasher = new PasswordHasher<UserDto>();
        var result = hasher.VerifyHashedPassword(userDto, storedHash!, password);

        return result == PasswordVerificationResult.Success;
    }

    public async Task<bool> DeleteUserAsync(string userId, string? photo, CancellationToken ct = default)
    {
        var cmd = new SqlCommand();
        cmd.CommandText = DeleteAspNetUserCommand;
        cmd.Parameters.AddWithValue("@UserId", userId);
        try
        {
            var scalar = await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
            if (scalar)
            {
                if (!string.IsNullOrEmpty(photo) && !string.IsNullOrWhiteSpace(photo))
                {
                    await storageService.DeleteFileAsync(Path.Combine(_userFolder, photo));
                }
            }

            return scalar;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<UserDto> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = FindAspNetUserByEmailCommand;
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

    public async Task<UserDto> FindUserByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = FindAspNetUserByUserIdCommand;
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
                    LockoutEnd = rdr.IsDBNull(rdr.GetOrdinal("LockoutEnd")) ? (DateTimeOffset?)null : rdr.GetFieldValue<DateTimeOffset>(rdr.GetOrdinal("LockoutEnd")),
                    Photo = rdr.IsDBNull(rdr.GetOrdinal("Photo")) ? "placeholder.png" : rdr.GetString(rdr.GetOrdinal("Photo"))
                };
            }

            return null!;
        }
        catch (SqlException ex) when (ex.Number == 500000)
        {
            return null!;
        }
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        await using var conn = await AppDbContext.Instance.GetOpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = GetAllAspNetUsersCommand;
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
        cmd.CommandText = GetAspNetUserRolesCommand;
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

    public async Task<bool> UpdateUserAsync(UserDto user, CancellationToken ct = default)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        if (user.PhotoFile is not null || user.PhotoFile?.Length > 0)
        {
            if (!string.Equals(user.PhotoFile?.FileName, "placeholder.png", StringComparison.OrdinalIgnoreCase))
                user.Photo = await storageService.SavePhotoAsync(user.PhotoFile!, _userFolder);
        }

        var cmd = new SqlCommand();
        cmd.CommandText = UpdateAspNetUserCommand;

        cmd.Parameters.AddWithValue("@UserId", user.UserId);
        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
        cmd.Parameters.AddWithValue("@LastName", user.LastName);
        cmd.Parameters.AddWithValue("@Gender", user.Gender);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
        cmd.Parameters.AddWithValue("@Photo", user.Photo == "placeholder.png" ? (object)DBNull.Value : user.Photo);

        try
        {
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UserByExistEmail(string email, string? userId, CancellationToken ct = default)
    {
        var cmd = new SqlCommand();
        cmd.CommandText = UserByExistEmailCommand;
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@UserId", userId);

        try
        {
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
