﻿using BlogApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Serilog;
using Serilog.Core;

namespace BlogApp.Api.Tests
{
    /// <summary>
    /// A collection fixture that is responsible for creating and dropping the database
    /// https://xunit.net/docs/shared-context
    /// </summary>
    public class DbFixture : IDisposable
    {
        private readonly BlogDbContext _dbContext;
        public readonly string BlogDbName = $"Blog-{Guid.NewGuid()}";
        public readonly string ConnString;
        
        
        private bool _disposed;

        public DbFixture()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Debug("Getting environment variables...");
            ConnString = Environment.GetEnvironmentVariable("DBCONNSTRING");
            ConnString = ConnString?.Replace("{BlogDbName}", BlogDbName);

            var builder = new DbContextOptionsBuilder<BlogDbContext>();

            builder.UseSqlServer(ConnString);
            _dbContext = new BlogDbContext(builder.Options);

            _dbContext.Database.Migrate();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // remove the temp db from the server once all tests are done
                    _dbContext.Database.EnsureDeleted();
                }

                _disposed = true;
            }
        }
    }

    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DbFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
