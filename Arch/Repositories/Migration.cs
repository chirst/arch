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
    const string userTable = """
        CREATE TABLE IF NOT EXISTS user (
            Id TEXT NOT NULL,
            UserName TEXT,
            NormalizedUserName TEXT,
            PasswordHash TEXT
        );
        """;

    // Id TEXT NOT NULL,
    // UserName TEXT,
    // NormalizedUserName TEXT,
    // PasswordHash TEXT,
    // Email TEXT,
    // NormalizedEmail TEXT,
    // EmailConfirmed INTEGER NOT NULL,
    // SecurityStamp TEXT,
    // ConcurrencyStamp TEXT,
    // PhoneNumber TEXT,
    // PhoneNumberConfirmed INTEGER NOT NULL,
    // TwoFactorEnabled INTEGER NOT NULL,
    // LockoutEnd TEXT,
    // LockoutEnabled INTEGER NOT NULL,
    // AccessFailedCount INTEGER NOT NULL,
    // UserType INTEGER NOT NULL,
    // IsActive INTEGER NOT NULL,

    public void Execute() => Connection(c => c.Execute(userTable));
}
