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
public class DbContextSubjectTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Subject_Persisted()
    {
        //Arrange
        SubjectEntity entity = new()
        {
            Id = Guid.Parse("C5DE45D7-64A0-4E8D-AC7F-BF5CFDFB0EFC"),
            Name = "Dějepis",
            Abbreviation = "D"
        };

        //Act
        InformationSystemDbContextSUT.Subjects.Add(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.Subjects.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntities);
    }

    [Fact]
    public async Task GetAll_Subjects_ContainsSeededSubject()
    {
        //Act
        var entities = await InformationSystemDbContextSUT.Subjects.ToArrayAsync();

        //Assert
        DeepAssert.Contains(SubjectSeeds.SubjectEntity0, entities);
    }

    [Fact]
    public async Task GetById_Subject_SubjectRetrieved()
    {
        //Act
        var entity = await InformationSystemDbContextSUT.Subjects.SingleAsync(i => i.Id == SubjectSeeds.SubjectEntity1.Id);

        //Assert
        var checkEntity = SubjectSeeds.SubjectEntity1
            with { Students = Array.Empty<EnrollmentEntity>(), Activities = Array.Empty<ActivityEntity>()};
        DeepAssert.Equal(checkEntity, entity);
    }

    [Fact]
    public async Task Update_Subject_Persisted()
    {
        //Arrange
        var baseEntity = SubjectSeeds.SubjectUpdate;
        var entity =
            baseEntity with
            {
                Name = baseEntity + "Updated",
                Abbreviation = baseEntity + "Updated",
            };

        //Act
        InformationSystemDbContextSUT.Subjects.Update(entity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Subjects.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    [Fact]
    public async Task Delete_Subject_SubjectDeleted()
    {
        //Arrange
        var entityBase = SubjectSeeds.SubjectDelete;

        //Act
        InformationSystemDbContextSUT.Subjects.Remove(entityBase);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Subjects.AnyAsync(i => i.Id == entityBase.Id));
    }

    [Fact]
    public async Task DeleteById_Subject_SubjectDeleted()
    {
        //Arrange
        var entityBase = SubjectSeeds.SubjectDelete;

        //Act
        InformationSystemDbContextSUT.Remove(
            InformationSystemDbContextSUT.Subjects.Single(i => i.Id == entityBase.Id));
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Subjects.AnyAsync(i => i.Id == entityBase.Id));
    }

    [Fact]
    public async Task Delete_SubjectWithActivityEntities_Deleted()
    {
        //Arrange
        var baseEntity = SubjectSeeds.SubjectForActivityEntityDelete;

        //Act
        InformationSystemDbContextSUT.Subjects.Remove(baseEntity);
        await InformationSystemDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await InformationSystemDbContextSUT.Subjects.AnyAsync(i => i.Id == baseEntity.Id));
        Assert.False(await InformationSystemDbContextSUT.Activities
            .AnyAsync(i => baseEntity.Activities.Select(Activity => Activity.Id).Contains(i.Id)));
    }
}
