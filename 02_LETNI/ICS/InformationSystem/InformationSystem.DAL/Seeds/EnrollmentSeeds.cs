using InformationSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL.Seeds;

public static class EnrollmentSeeds
{
    public static readonly EnrollmentEntity EmptyEnrollmentEntity = new()
    {
        Id = default,
        Subject = default!,
        SubjectId = default,
        Student = default!,
        StudentId = default
    };

    public static readonly EnrollmentEntity EnrollmentEntity0 = new()
    {
        Id = Guid.Parse(input: "0d4fc150-ad80-4d46-a511-4c666166ec5e"),
        Subject = SubjectSeeds.SubjectEntity1,
        SubjectId = SubjectSeeds.SubjectEntity1.Id,
        Student = StudentSeeds.StudentEntity1,
        StudentId = StudentSeeds.StudentEntity1.Id,
    };

    public static readonly EnrollmentEntity EnrollmentEntity1 = new()
    {
        Id = Guid.Parse(input: "0d4fa150-ad80-4d46-a511-4c666166ec5e"),
        Subject = SubjectSeeds.SubjectEntity1,
        SubjectId = SubjectSeeds.SubjectEntity1.Id,
        Student = StudentSeeds.StudentEntity2,
        StudentId = StudentSeeds.StudentEntity2.Id,
    };

    public static readonly EnrollmentEntity EnrollmentEntity2 = new()
    {
        Id = Guid.Parse(input: "87833e66-05ba-4d6b-900b-fe5ace88dbd8"),
        Subject = SubjectSeeds.SubjectEntity2,
        SubjectId = SubjectSeeds.SubjectEntity2.Id,
        Student = StudentSeeds.StudentEntity2,
        StudentId = StudentSeeds.StudentEntity2.Id,
    };

    //To ensure that no tests reuse these clones for non-idempotent operations
    public static readonly EnrollmentEntity EnrollmentEntityUpdate = EnrollmentEntity0 with { Id = Guid.Parse("A2E6849D-A158-4436-980C-7FC26B60C674"), Subject = null!, SubjectId = SubjectSeeds.SubjectForActivityEntityUpdate.Id, Student = null!, StudentId = StudentSeeds.StudentUpdate.Id };
    public static readonly EnrollmentEntity EnrollmentEntityDelete = EnrollmentEntity0 with { Id = Guid.Parse("30872EFF-CED4-4F2B-89DB-0EE83A74D279"), Subject = null!, SubjectId = SubjectSeeds.SubjectForActivityEntityDelete.Id, Student = null!, StudentId = StudentSeeds.StudentDelete.Id };

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EnrollmentEntity>().HasData(
            EnrollmentEntity0 with { Student = null!, Subject = null!},
            EnrollmentEntity1 with { Student = null!, Subject = null! },
            EnrollmentEntity2 with { Student = null!, Subject = null! },
            EnrollmentEntityUpdate,
            EnrollmentEntityDelete
        );
    }
}

