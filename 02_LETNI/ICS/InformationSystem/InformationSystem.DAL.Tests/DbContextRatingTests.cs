using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace InformationSystem.DAL.Tests;

public class DbContextRatingTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task GetAll_Ratings_ForActivity()
    {
        //Act
        var activities = await InformationSystemDbContextSUT.Ratings
            .Where(i => i.ActivityId == ActivitySeeds.ActivityEntity1.Id)
            .ToArrayAsync();

        //Assert
        DeepAssert.Contains(RatingSeeds.RatingEntity1 with { Activity = null!, Student = null! }, activities);
        DeepAssert.Contains(RatingSeeds.RatingEntity2 with { Activity = null!, Student = null! }, activities);
    }


    [Fact]
    public async Task Update_RatingValue_Persisted()
    {
        //Arrange
        var baseEntity = RatingSeeds.RatingEntityUpdate;
        var entity =
            baseEntity with
            {
                Points=1.0f,
            };

        //Act
        InformationSystemDbContextSUT.Ratings.Update(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Ratings.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    [Fact]
    public async Task Delete_Rating_Deleted()
    {
        //Arrange
        var baseEntity = RatingSeeds.RatingEntityDelete;

        //Act
        InformationSystemDbContextSUT.Ratings.Remove(baseEntity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Ratings.AnyAsync(i => i.Id == baseEntity.Id));
    }

    [Fact]
    public async Task DeleteById_Rating_Deleted()
    {
        //Arrange
        var baseEntity = RatingSeeds.RatingEntityDelete;

        //Act
        InformationSystemDbContextSUT.Remove(
            InformationSystemDbContextSUT.Ratings.Single(i => i.Id == baseEntity.Id));
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Ratings.AnyAsync(i => i.Id == baseEntity.Id));
    }
}
