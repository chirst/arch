using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Arch.Repositories;

public class UserRepository(ILogger<UserRepository> logger)
    : ArchRepository,
        IUserStore<IdentityUser>,
        IUserPasswordStore<IdentityUser>
{
    public Task<IdentityResult> CreateAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            Result
                .Try(() =>
                {
                    logger.LogInformation("creating user");
                    return c.ExecuteAsync(
                        """
                        INSERT INTO user (Id, UserName, NormalizedUserName, PasswordHash)
                        VALUES (@Id, @UserName, @NormalizedUserName, @PasswordHash)
                        """,
                        user
                    );
                })
                .Finally(r =>
                {
                    logger.LogInformation("logged user");
                    if (r.IsFailure)
                        logger.LogError(
                            "failed to create user {error}",
                            r.Error
                        );
                    return r.IsSuccess
                        ? IdentityResult.Success
                        : IdentityResult.Failed();
                })
        );

    public Task<IdentityResult> DeleteAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public void Dispose() { }

    public Task<IdentityUser?> FindByIdAsync(
        string userId,
        CancellationToken cancellationToken
    ) =>
        Connection(c =>
            c.QuerySingleOrDefaultAsync<IdentityUser?>(
                """
                SELECT * FROM user
                WHERE Id = @Id
                """,
                new { Id = userId }
            )
        );

    public Task<IdentityUser?> FindByNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("find {username}", normalizedUserName);
        return Connection(c =>
            c.QuerySingleOrDefaultAsync<IdentityUser?>(
                """
                SELECT * FROM user
                WHERE NormalizedUserName = @normalizedUserName
                """,
                new { normalizedUserName }
            )
        );
    }

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
                SELECT PasswordHash FROM user
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
                SELECT Id FROM user
                WHERE Id = @Id
                """,
                user
            )
        );

    public Task<string?> GetUserNameAsync(
        IdentityUser user,
        CancellationToken cancellationToken
    ) => Task.FromResult(user.UserName);

    public Task<Result<IEnumerable<IdentityUser>>> GetUsers() =>
        Connection(c =>
            Result.Try(
                () => c.QueryAsync<IdentityUser>("""SELECT * FROM user;""")
            )
        );

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
    )
    {
        throw new NotImplementedException();
    }
}
