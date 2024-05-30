using InformationSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace InformationSystem.DAL.Seeds;

public static class StudentSeeds
{

    public static readonly StudentEntity EmptyStudent = new()
    {
        Id = default,
        Name = default!,
        Surname = default!,
        PhotoUrl = default
    };

    public static readonly StudentEntity StudentEntity0 = new()
    {
        Id = Guid.Parse(input: "06a8a2cf-ea03-4095-a3e4-aa0291fe9c75"),
        Name = "Honza",
        Surname = "Krecek",
        PhotoUrl = "https://www.pngitem.com/pimgs/m/40-406527_cartoon-glass-of-water-png-glass-of-water.png",
    };

    //To ensure that no tests reuse these clones for non-idempotent operations
    public static readonly StudentEntity StudentUpdate = StudentEntity0 with { Id = Guid.Parse("143332B9-080E-4953-AEA5-BEF64679B052") };
    public static readonly StudentEntity StudentDelete = StudentEntity0 with { Id = Guid.Parse("274D0CC9-A948-4818-AADB-A8B4C0506619") };

    public static StudentEntity StudentEntity1 = new()
    {
        Id = Guid.Parse(input: "df935095-8709-4040-a2bb-b6f97cb416dc"),
        Name = "Franta",
        Surname = "Siska",
        PhotoUrl = null
    };

    public static StudentEntity StudentEntity2 = new()
    {
        Id = Guid.Parse(input: "23b3902d-7d4f-4213-9cf0-112348f56238"),
        Name = "Petr",
        Surname = "Narozny",
        PhotoUrl = null
    };

    static StudentSeeds()
    {
        StudentEntity1.Subjects.Add(EnrollmentSeeds.EnrollmentEntity0);
        StudentEntity2.Subjects.Add(EnrollmentSeeds.EnrollmentEntity1);
        StudentEntity2.Subjects.Add(EnrollmentSeeds.EnrollmentEntity2);
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentEntity>().HasData(
            StudentEntity0,
            StudentEntity1 with { Subjects = Array.Empty<EnrollmentEntity>()},
            StudentEntity2 with { Subjects = Array.Empty<EnrollmentEntity>()},
            StudentUpdate,
            StudentDelete
            );
    }
}
