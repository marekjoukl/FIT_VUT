using InformationSystem.Common.Enums;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace InformationSystem.DAL.Tests;

public class DbContextActivityTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task GetAll_Activities_ForSubject()
    {
        //Act
        var activities = await InformationSystemDbContextSUT.Activities
            .Where(i => i.SubjectId == SubjectSeeds.SubjectEntity1.Id)
            .ToArrayAsync();

        //Assert
        var compare = ActivitySeeds.ActivityEntity1;
        DeepAssert.Contains(ActivitySeeds.ActivityEntity1 , activities);
        // DeepAssert.Contains(ActivitySeeds.ActivityEntity2 with { Subject = null!, Ratings = null! }, activities);
    }

    [Fact]
    public async Task Update_ActivityRoom_Persisted()
    {
        //Arrange
        var baseEntity = ActivitySeeds.ActivityEntityUpdate;
        var entity =
            baseEntity with
            {
                Room = ActivityRoom.A5,
            };

        //Act
        InformationSystemDbContextSUT.Activities.Update(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Activities.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    [Fact]
    public async Task Delete_Activity_Deleted()
    {
        //Arrange
        var baseEntity = ActivitySeeds.ActivityEntityDelete;

        //Act
        InformationSystemDbContextSUT.Activities.Remove(baseEntity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Activities.AnyAsync(i => i.Id == baseEntity.Id));
    }

    [Fact]
    public async Task DeleteById_Activity_Deleted()
    {
        //Arrange
        var baseEntity = ActivitySeeds.ActivityEntityDelete;

        //Act
        InformationSystemDbContextSUT.Remove(
            InformationSystemDbContextSUT.Activities.Single(i => i.Id == baseEntity.Id));
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Activities.AnyAsync(i => i.Id == baseEntity.Id));
    }
}
