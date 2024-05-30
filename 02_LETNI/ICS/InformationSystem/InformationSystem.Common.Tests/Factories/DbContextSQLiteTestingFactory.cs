﻿using InformationSystem.DAL;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.Common.Tests.Factories;

public class DbContextSqLiteTestingFactory(string databaseName, bool seedTestingData = false)
    : IDbContextFactory<InformationSystemDbContext>
{
    public InformationSystemDbContext CreateDbContext()
    {
        DbContextOptionsBuilder<InformationSystemDbContext> builder = new();
        builder.UseSqlite($"Data Source={databaseName};Cache=Shared");

        // builder.LogTo(Console.WriteLine); //Enable in case you want to see tests details, enabled may cause some inconsistencies in tests
        // builder.EnableSensitiveDataLogging();

        return new InformationSystemTestingDbContext(builder.Options, seedTestingData);
    }
}
