using System;

namespace Post.Query.Infrastructure.DataAccess
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsbuilder> _configureDbContext;
    }

    public DatabaseContextFactory(Action<DbContextOptionsbuilder> configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }

    public DatabaseContext Create()
    {
        DbContextOptionsbuilder<DatabaseContext> options = new();
        _configureDbContext(options);

        return new DatabaseContext(options.Options);
    }
}
