using InformationSystem.BL.Mappers;
using InformationSystem.Common.Tests;
using InformationSystem.Common.Tests.Factories;
using InformationSystem.DAL;
using InformationSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace InformationSystem.BL.Tests;

public class FacadeTestsBase : IAsyncLifetime
{
    protected FacadeTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        // DbContextFactory = new DbContextTestingInMemoryFactory(GetType().Name, seedTestingData: true);
        // DbContextFactory = new DbContextLocalDBTestingFactory(GetType().FullName!, seedTestingData: true);
        DbContextFactory = new DbContextSqLiteTestingFactory(GetType().FullName!, seedTestingData: true);

        EnrollmentModelMapper = new EnrollmentModelMapper();
        RatingModelMapper = new RatingModelMapper();
        SubjectModelModelMapper = new SubjectModelModelMapper();
        StudentModelMapper = new StudentModelMapper(EnrollmentModelMapper);
        ActivityModelMapper = new ActivityModelMapper(RatingModelMapper, SubjectModelModelMapper);

        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);
        InformationSystemDbContextSUT = DbContextFactory.CreateDbContext();

    }

    protected IDbContextFactory<InformationSystemDbContext> DbContextFactory { get; }

    protected InformationSystemDbContext InformationSystemDbContextSUT { get; }

    protected StudentModelMapper StudentModelMapper { get; }
    protected EnrollmentModelMapper EnrollmentModelMapper { get; }
    protected SubjectModelModelMapper SubjectModelModelMapper { get; }
    protected RatingModelMapper RatingModelMapper { get; }
    protected ActivityModelMapper ActivityModelMapper { get; }
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }

    public async Task InitializeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
    }
}

