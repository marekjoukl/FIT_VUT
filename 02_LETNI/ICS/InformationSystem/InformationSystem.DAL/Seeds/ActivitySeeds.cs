using InformationSystem.Common.Enums;
using InformationSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL.Seeds;

public static class ActivitySeeds
{
    public static readonly ActivityEntity EmptyIngredientAmountEntity = new()
    {
        Id = default,
        Description = default,
        Start = default,
        End = default,
        Room = default,
        Type = default,
        Subject = default!,
        SubjectId = default,
    };

    public static readonly ActivityEntity ActivityEntity1 = new()
    {
        Id = Guid.Parse(input: "0d4fa150-ad80-4d46-a511-4c666166ec5e"),
        Description = "terrible activity",
        Start = new DateTime(2012, 12, 25, 10, 30, 0),
        End = new DateTime(2012, 12, 25, 11, 30, 0),
        Room = ActivityRoom.A1,
        Type = ActivityType.Seminar,
        SubjectId = SubjectSeeds.SubjectEntity1.Id,
        Subject = SubjectSeeds.SubjectEntity1
    };

    public static readonly ActivityEntity ActivityEntity2 = new()
    {
        Id = Guid.Parse(input: "87833e66-05ba-4d6b-900b-fe5ace88dbd8"),
        Description = "terrible activity",
        Start = new DateTime(2012, 12, 25, 10, 30, 0),
        End = new DateTime(2012, 12, 25, 11, 30, 0),
        Room = ActivityRoom.A1,
        Type = ActivityType.Seminar,
        SubjectId = SubjectSeeds.SubjectEntity2.Id,
        Subject = SubjectSeeds.SubjectEntity2
    };

    //To ensure that no tests reuse these clones for non-idempotent operations
    public static readonly ActivityEntity ActivityEntityUpdate = ActivityEntity1 with { Id = Guid.Parse("A2E6849D-A158-4436-980C-7FC26B60C674"), Subject = null!, SubjectId = SubjectSeeds.SubjectForActivityEntityUpdate.Id, Ratings = Array.Empty<RatingEntity>() };
    public static readonly ActivityEntity ActivityEntityDelete = ActivityEntity1 with { Id = Guid.Parse("30872EFF-CED4-4F2B-89DB-0EE83A74D279"), Subject = null!, SubjectId = SubjectSeeds.SubjectForActivityEntityDelete.Id, Ratings = Array.Empty<RatingEntity>() };

    static ActivitySeeds()
    {
        ActivityEntity1.Subject = SubjectSeeds.SubjectEntity1;
        ActivityEntity2.Subject = SubjectSeeds.SubjectEntity2;
        ActivityEntity1.Ratings.Add(RatingSeeds.RatingEntity1);
        ActivityEntity1.Ratings.Add(RatingSeeds.RatingEntity2);
        ActivityEntity2.Ratings.Add(RatingSeeds.RatingEntity0);
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityEntity>().HasData(
            ActivityEntity1 with { Subject = null!, Ratings = Array.Empty<RatingEntity>()},
            ActivityEntity2 with { Subject = null!, Ratings = Array.Empty<RatingEntity>() },
            ActivityEntityUpdate,
            ActivityEntityDelete
        );
    }
}

