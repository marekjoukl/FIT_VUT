using InformationSystem.DAL;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.Common.Tests;

public class InformationSystemTestingDbContext(DbContextOptions contextOptions, bool seedTestingData = false)
    : InformationSystemDbContext(contextOptions, seedDemoData: false)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (seedTestingData)
        {
            StudentSeeds.Seed(modelBuilder);
            SubjectSeeds.Seed(modelBuilder);
            EnrollmentSeeds.Seed(modelBuilder);
            ActivitySeeds.Seed(modelBuilder);
            RatingSeeds.Seed(modelBuilder);
        }
    }
}
