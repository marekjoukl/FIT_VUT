using InformationSystem.DAL.Entities;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace InformationSystem.DAL.Tests;

/// <summary>
/// Tests shows an example of DbContext usage when querying strong entity with no navigation properties.
/// Entity has no relations, holds no foreign keys.
/// </summary>
public class DbContextStudentTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Student_Persisted()
    {
        //Arrange
        StudentEntity entity = new()
        {
            Id = Guid.Parse("C5DE45D7-64A0-4E8D-AC7F-BF5CFDFB0EFC"),
            Name = "Miloš",
            Surname = "Zeman",
            PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/78/Salt_shaker_on_white_background.jpg/800px-Salt_shaker_on_white_background.jpg"
        };

        //Act
        InformationSystemDbContextSUT.Students.Add(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.Students.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntities);
    }

    [Fact]
    public async Task GetAll_Students_ContainsSeededStudent()
    {
        //Act
        var entities = await InformationSystemDbContextSUT.Students.ToArrayAsync();

        //Assert
        DeepAssert.Contains(StudentSeeds.StudentEntity0, entities);
    }

    [Fact]
    public async Task GetById_Student_StudentRetrieved()
    {
        //Act
        var entity = await InformationSystemDbContextSUT.Students.SingleAsync(i=>i.Id == StudentSeeds.StudentEntity1.Id);

        //Assert
        var checkEntity = StudentSeeds.StudentEntity1 with { Subjects = Array.Empty<EnrollmentEntity>() };
        DeepAssert.Equal(checkEntity, entity);
    }

    [Fact]
    public async Task Update_Student_Persisted()
    {
        //Arrange
        var baseEntity = StudentSeeds.StudentUpdate;
        var entity =
            baseEntity with
            {
                Name = baseEntity + "Updated",
                Surname = baseEntity + "Updated",
            };

        //Act
        InformationSystemDbContextSUT.Students.Update(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Students.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    [Fact]
    public async Task Delete_Student_StudentDeleted()
    {
        //Arrange
        var entityBase = StudentSeeds.StudentDelete;

        //Act
        InformationSystemDbContextSUT.Students.Remove(entityBase);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Students.AnyAsync(i => i.Id == entityBase.Id));
    }

    [Fact]
    public async Task DeleteById_Student_StudentDeleted()
    {
        //Arrange
        var entityBase = StudentSeeds.StudentDelete;

        //Act
        InformationSystemDbContextSUT.Remove(
            InformationSystemDbContextSUT.Students.Single(i => i.Id == entityBase.Id));
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Students.AnyAsync(i => i.Id == entityBase.Id));
    }
    /*
    [Fact]
    public async Task Delete_IngredientUsedInRecipe_Throws()
    {
        //Arrange
        var entityBase = IngredientSeeds.IngredientEntity1;

        //Act & Assert
        CookBookDbContextSUT.Ingredients.Remove(entityBase);
        await Assert.ThrowsAsync<DbUpdateException>(async () => await CookBookDbContextSUT.SaveChangesAsync());
    }*/
}
