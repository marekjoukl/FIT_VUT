using InformationSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL.Seeds;

public static class SubjectSeeds
{
    public static readonly SubjectEntity EmptySubject= new()
    {
        Id = default,
        Name = "default",
        Abbreviation = "default",
    };

    public static readonly SubjectEntity SubjectEntity0 = new()
    {
        Id = Guid.Parse("06f8a2cf-ea03-4095-a3e4-aa0291fe9c75"),
        Name = "Uzivatelska rozhrani",
        Abbreviation = "ITU",
        Activities = new List<ActivityEntity>()
    };

    //To ensure that no tests reuse these clones for non-idempotent operations
    public static readonly SubjectEntity SubjectUpdate = SubjectEntity0 with { Id = Guid.Parse("143332B9-080F-4953-AEA5-BEF64679B052"), Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() };
    public static readonly SubjectEntity SubjectDelete = SubjectEntity0 with { Id = Guid.Parse("274D0CC9-F948-4818-AADB-A8B4C0506619"), Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() };
    public static readonly SubjectEntity SubjectForActivityEntityUpdate = SubjectEntity0 with { Id = Guid.Parse("4FD824C0-F7D1-48BA-8E7C-4F136CF8BF31"), Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() };
    public static readonly SubjectEntity SubjectForActivityEntityDelete = SubjectEntity0 with { Id = Guid.Parse("F78ED923-F094-4016-9045-3F5BB7F2EB88"), Activities = new List<ActivityEntity>(), Students = new List<EnrollmentEntity>(), Abbreviation = "IPP"};

    public static SubjectEntity SubjectEntity1 = new()
    {
        Id = Guid.Parse(input: "ff935095-8709-4040-a2bb-b6f97cb416dc"),
        Name = "Algoritmy",
        Abbreviation = "IAL",
        // Activities = new List<ActivityEntity>()
    };

    public static SubjectEntity SubjectEntity2 = new()
    {
        Id = Guid.Parse(input: "23f3902d-7d4f-4213-9cf0-112348f56238"),
        Name = "Matematika",
        Abbreviation = "IMA",
        // Activities = new List<ActivityEntity>()
    };

    static SubjectSeeds()
    {
        SubjectEntity1.Activities.Add(ActivitySeeds.ActivityEntity1);
        SubjectEntity2.Activities.Add(ActivitySeeds.ActivityEntity2);
        SubjectEntity1.Students.Add(EnrollmentSeeds.EnrollmentEntity0);
        SubjectEntity1.Students.Add(EnrollmentSeeds.EnrollmentEntity1);
        SubjectEntity2.Students.Add(EnrollmentSeeds.EnrollmentEntity2);
        SubjectForActivityEntityDelete.Activities.Add(ActivitySeeds.ActivityEntityDelete);
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubjectEntity>().HasData(
            SubjectEntity0 with { Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() },
            SubjectEntity1 with { Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() },
            SubjectEntity2 with { Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() },
            SubjectUpdate,
            SubjectDelete,
            SubjectForActivityEntityUpdate,
            SubjectForActivityEntityDelete with { Activities = Array.Empty<ActivityEntity>(), Students = Array.Empty<EnrollmentEntity>() }
            );
    }
}
