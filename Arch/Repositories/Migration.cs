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
    public void Execute() =>
        Connection(c =>
            c.Execute(
                """
                CREATE TABLE IF NOT EXISTS user (
                    Id TEXT NOT NULL,
                    UserName TEXT,
                    NormalizedUserName TEXT,
                    PasswordHash TEXT
                );
                """
            )
        );
}
