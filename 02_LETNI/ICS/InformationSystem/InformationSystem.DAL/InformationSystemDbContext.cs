using InformationSystem.DAL.Entities;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL;

public class InformationSystemDbContext(DbContextOptions contextOptions, bool seedDemoData = false) : DbContext(contextOptions)
{
    public DbSet<StudentEntity> Students => Set<StudentEntity>();
    public DbSet<SubjectEntity> Subjects => Set<SubjectEntity>();
    public DbSet<EnrollmentEntity> Enrollments => Set<EnrollmentEntity>();
    public DbSet<ActivityEntity> Activities => Set<ActivityEntity>();
    public DbSet<RatingEntity> Ratings => Set<RatingEntity>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentEntity>()
            .HasMany(i => i.Subjects)
            .WithOne(i => i.Student)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubjectEntity>()
            .HasMany(i => i.Students)
            .WithOne(i => i.Subject)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubjectEntity>()
            .HasMany(i => i.Activities)
            .WithOne(i => i.Subject)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActivityEntity>()
            .HasOne(i => i.Subject)
            .WithMany(i => i.Activities);

        if (seedDemoData)
        {
            SubjectSeeds.Seed(modelBuilder);
            StudentSeeds.Seed(modelBuilder);
            EnrollmentSeeds.Seed(modelBuilder);
            ActivitySeeds.Seed(modelBuilder);
            RatingSeeds.Seed(modelBuilder);
        }
    }
}
