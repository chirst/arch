using Dapper;

namespace Arch.Repositories;

public static class MigrationExtensions
{
    public static IHost Migrate(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        scope
            .ServiceProvider.GetRequiredService<MigrationRepository>()
            .Execute();
        return host;
    }
}

public class MigrationRepository : ArchRepository
{
    // IdentityUser has non standard naming to make it easier to map to the
    // AspNetCore IdentityUser model.
    private const string userTable = """
        CREATE TABLE IF NOT EXISTS IdentityUser (
            Id TEXT NOT NULL,
            UserName TEXT,
            NormalizedUserName TEXT,
            PasswordHash TEXT,
            LockoutEnabled INT NOT NULL,
            AccessFailedCount INT NOT NULL,
            LockoutEnd DATETIME
        );
        """;

    public void Execute() => Connection(c => c.Execute(userTable));
}
