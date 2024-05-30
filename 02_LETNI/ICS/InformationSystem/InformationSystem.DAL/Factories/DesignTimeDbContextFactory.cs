using InformationSystem.DAL.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

////////////////////////////
// Copy-pasted from CookBook
////////////////////////////

namespace InformationSystem.DAL.Factories;

/// <summary>
///     EF Core CLI migration generation uses this DbContext to create model and migration
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InformationSystemDbContext>
{
    private readonly DbContextSqLiteFactory _dbContextSqLiteFactory = new($"Data Source=InformationSystem;Cache=Shared");

    public InformationSystemDbContext CreateDbContext(string[] args) => _dbContextSqLiteFactory.CreateDbContext();
}
