using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Arch.Repositories;

public class UserRepository(ILogger<UserRepository> logger)
    : ArchRepository,
        IUserStore<IdentityUser>,
        IUserPasswordStore<IdentityUser>,
        IUserLockoutStore<IdentityUser>
{
    public Task<IdentityResult> CreateAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            Result
                .Try(
                    () =>
                        c.ExecuteAsync(
                            """
                            INSERT INTO IdentityUser (
                                Id, 
                                UserName,
                                NormalizedUserName,
                                PasswordHash,
                                LockoutEnabled,
                                AccessFailedCount
                            )
                            VALUES (
                                @Id,
                                @UserName,
                                @NormalizedUserName,
                                @PasswordHash,
                                @LockoutEnabled,
                                @AccessFailedCount
                            )
                            """,
                            user
                        )
                )
                .Finally(r =>
                {
                    if (r.IsSuccess)
                        return IdentityResult.Success;
                    logger.LogError(r.Error);
                    return IdentityResult.Failed();
                })
        );

    // Should delete from database by user id.
    public Task<IdentityResult> DeleteAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();

    public void Dispose() { }

    public Task<IdentityUser?> FindByIdAsync(
        string userId,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            c.QuerySingleOrDefaultAsync<IdentityUser?>(
                """
                SELECT * FROM IdentityUser
                WHERE Id = @Id
                """,
                new { Id = userId }
            )
        );

    public Task<IdentityUser?> FindByNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            c.QuerySingleOrDefaultAsync<IdentityUser?>(
                """
                SELECT * FROM IdentityUser 
                WHERE NormalizedUserName = @normalizedUserName
                """,
                new { normalizedUserName }
            )
        );

    public Task<string?> GetNormalizedUserNameAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.NormalizedUserName);

    public Task<string?> GetPasswordHashAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            c.QuerySingleOrDefaultAsync<string?>(
                """
                SELECT PasswordHash FROM IdentityUser
                WHERE Id = @Id
                """,
                user
            )
        );

    public Task<string> GetUserIdAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            c.QuerySingleAsync<string>(
                """
                SELECT Id FROM IdentityUser
                WHERE Id = @Id
                """,
                user
            )
        );

    public Task<string?> GetUserNameAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.UserName);

    public Task<bool> HasPasswordAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    public Task SetNormalizedUserNameAsync(
        IdentityUser user,
        string? normalizedName,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.NormalizedUserName = normalizedName);

    public Task SetPasswordHashAsync(
        IdentityUser user,
        string? passwordHash,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.PasswordHash = passwordHash);

    public Task SetUserNameAsync(
        IdentityUser user,
        string? userName,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.UserName = userName);

    public Task<IdentityResult> UpdateAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            Result
                .Try(
                    () =>
                        c.ExecuteAsync(
                            """
                            UPDATE IdentityUser
                            SET Id = @Id,
                                LockoutEnabled = @LockoutEnabled,
                                AccessFailedCount = @AccessFailedCount,
                                LockoutEnd = @LockoutEnd
                            WHERE Id = @Id
                            """,
                            new
                            {
                                user.Id,
                                user.LockoutEnabled,
                                user.AccessFailedCount,
                                LockoutEnd = user.LockoutEnd?.ToString("o"),
                            }
                        )
                )
                .Finally(r =>
                {
                    if (r.IsSuccess)
                        return IdentityResult.Success;
                    logger.LogError(r.Error);
                    return IdentityResult.Failed();
                })
        );

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.LockoutEnd);

    public Task SetLockoutEndDateAsync(
        IdentityUser user,
        DateTimeOffset? lockoutEnd,
        CancellationToken cancellationToken
    )
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task<int> IncrementAccessFailedCountAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("before count {user}", user.AccessFailedCount);
        user.AccessFailedCount++;
        logger.LogInformation(
            "incremented count to {user}",
            user.AccessFailedCount
        );
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task ResetAccessFailedCountAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    )
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task<int> GetAccessFailedCountAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<bool> GetLockoutEnabledAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    public Task SetLockoutEnabledAsync(
        IdentityUser user,
        bool enabled,
        CancellationToken cancellationToken
    )
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }
}
