using System.Data.SQLite;

namespace Arch.Repositories;

public abstract class ArchRepository
{
    public SQLiteConnection Connection() => new("DataSource=Arch.db");

    public T Connection<T>(Func<SQLiteConnection, T> callback) =>
        callback(Connection());
}
