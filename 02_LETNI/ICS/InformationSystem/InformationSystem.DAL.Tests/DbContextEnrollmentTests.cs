using InformationSystem.DAL.Entities;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace InformationSystem.DAL.Tests;

public class DbContextEnrollmentTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task GetAll_Enrollments_ForSubject()
    {
        //Act
        var activities = await InformationSystemDbContextSUT.Enrollments
            .Where(i => i.SubjectId == SubjectSeeds.SubjectEntity0.Id)
            .ToArrayAsync();

        //Assert
        DeepAssert.Contains(EnrollmentSeeds.EnrollmentEntity1 with { Subject = null!, Student = null! }, activities);
        DeepAssert.Contains(EnrollmentSeeds.EnrollmentEntity2 with { Subject = null!, Student = null! }, activities);
    }


    [Fact]
    public async Task Delete_Enrollment_Deleted()
    {
        //Arrange
        var baseEntity = EnrollmentSeeds.EnrollmentEntityDelete;

        //Act
        InformationSystemDbContextSUT.Enrollments.Remove(baseEntity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Enrollments.AnyAsync(i => i.Id == baseEntity.Id));
    }

    [Fact]
    public async Task DeleteById_Enrollment_Deleted()
    {
        //Arrange
        var baseEntity = EnrollmentSeeds.EnrollmentEntityDelete;

        //Act
        InformationSystemDbContextSUT.Remove(
            InformationSystemDbContextSUT.Enrollments.Single(i => i.Id == baseEntity.Id));
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Enrollments.AnyAsync(i => i.Id == baseEntity.Id));
    }
}
