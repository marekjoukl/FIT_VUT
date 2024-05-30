using InformationSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL.Seeds;

public static class RatingSeeds
{
    public static readonly RatingEntity EmptyRatingEntity = new()
    {
        Id = default,
        Points = default,
        Activity = default!,
        ActivityId = default,
        Student = default!,
        StudentId = default,
        Note = default
    };


    public static readonly RatingEntity RatingEntity0 = new()
    {
        Id = Guid.Parse(input: "bd41be4b-f9ae-4477-8582-f511ce879a7c"),
        Points = 2.0F,
        Activity = ActivitySeeds.ActivityEntity2,
        ActivityId = ActivitySeeds.ActivityEntity2.Id,
        Student = StudentSeeds.StudentEntity1,
        StudentId = StudentSeeds.StudentEntity1.Id,
        Note = "da se"
    };

    public static readonly RatingEntity RatingEntity1 = new()
    {
        Id = Guid.Parse(input: "7d6c8815-3b23-4a53-a9ae-83ddf0241a96"),
        Points = 4.0F,
        Activity = ActivitySeeds.ActivityEntity1,
        ActivityId = ActivitySeeds.ActivityEntity1.Id,
        Student = StudentSeeds.StudentEntity1,
        StudentId = StudentSeeds.StudentEntity1.Id,
        Note = "nic moc"
    };

    public static readonly RatingEntity RatingEntity2 = new()
    {
        Id = Guid.Parse(input: "87833e66-05ba-4d6b-900b-fe5ace88dbd8"),
        Points = 6.0F,
        Activity = ActivitySeeds.ActivityEntity1,
        ActivityId = ActivitySeeds.ActivityEntity1.Id,
        Student = StudentSeeds.StudentEntity2,
        StudentId = StudentSeeds.StudentEntity2.Id,
        Note = "vpohode"
    };

    //To ensure that no tests reuse these clones for non-idempotent operations
    public static readonly RatingEntity RatingEntityUpdate = RatingEntity0 with { Id = Guid.Parse("A2E6849D-A158-4436-980C-7FC26B60C674"), Activity = null!, ActivityId = ActivitySeeds.ActivityEntityUpdate.Id, Student= null!,StudentId=StudentSeeds.StudentUpdate.Id };
    public static readonly RatingEntity RatingEntityDelete = RatingEntity0 with { Id = Guid.Parse("30872EFF-CED4-4F2B-89DB-0EE83A74D279"), Activity = null!, ActivityId = ActivitySeeds.ActivityEntityDelete.Id, Student = null!, StudentId = StudentSeeds.StudentDelete.Id };

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RatingEntity>().HasData(
            RatingEntity0 with { Activity = null!, Student = null!},
            RatingEntity1 with { Activity = null!, Student = null! },
            RatingEntity2 with { Activity = null!, Student = null! },
            RatingEntityUpdate,
            RatingEntityDelete
        );
    }
}

