using System.Collections.ObjectModel;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.Common.Enums;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace InformationSystem.BL.Tests;

public class ActivityFacadeTests : FacadeTestsBase
{
    private readonly IActivityFacade _facadeSUT;

    public ActivityFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new ActivityFacade(UnitOfWorkFactory, ActivityModelMapper);
    }

    [Fact]
    public async Task Create_WithWithoutRating_EqualsCreated()
    {
        //Arrange
        var model = new ActivityDetailModel()
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Room = ActivityRoom.A5,
            Type = ActivityType.Lab,
            SubjectId = SubjectSeeds.SubjectEntity1.Id,
        };

        //Act
        var returnedModel = await _facadeSUT.SaveAsync(model);

        //Assert
        FixIds(model, returnedModel);
        DeepAssert.Equal(model, returnedModel);
    }

    [Fact]
    public async Task Create_WithNonExistingRating_Throws()
    {
        //Arrange
        var model = new ActivityDetailModel()
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Room = ActivityRoom.A5,
            Type = ActivityType.Seminar,
            SubjectId = SubjectSeeds.SubjectEntity1.Id,
            Subject = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity1),
            Students = new ObservableCollection<RatingListModel>()
            {
                new()
                {
                    Id = Guid.Empty,
                    Points = 0,
                    Note = "",
                    StudentId = Guid.Empty,
                    StudentName = "",
                    StudentSurname = ""
                }
            }
        };

        //Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingRating_Throws()
    {
        //Arrange
        var model = new ActivityDetailModel()
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Room = ActivityRoom.A5,
            Type = ActivityType.Seminar,
            SubjectId = SubjectSeeds.SubjectEntity1.Id,
            Subject = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity1),
            Students = new ObservableCollection<RatingListModel>()
            {
                new ()
                {
                    Points = (float)34.4,
                    Note = "Good job",
                    StudentId = StudentSeeds.StudentEntity0.Id,
                    StudentName = StudentSeeds.StudentEntity0.Name,
                    StudentSurname = StudentSeeds.StudentEntity0.Surname
                }
            },
        };

        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingAndNotExistingRating_Throws()
    {
        //Arrange
        var model = new ActivityDetailModel()
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(1),
            Room = ActivityRoom.A5,
            Type = ActivityType.Seminar,
            SubjectId = SubjectSeeds.SubjectEntity1.Id,
            Subject = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity1),
            Students =
            [
                new()
                {
                    Points = 0,
                    Note = "",
                    StudentId = Guid.Empty,
                    StudentName = "",
                    StudentSurname = ""
                },
                RatingModelMapper.MapToListModel(RatingSeeds.RatingEntity1)
            ]
        };

        //Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task GetById_FromSeeded_EqualsSeeded()
    {
        //Arrange
        var detailModel = ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1);

        //Act
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        //Assert
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task GetAll_FromSeeded_ContainsSeeded()
    {
        //Arrange & Act
        var entity = ActivitySeeds.ActivityEntity1;
        var check = ActivityModelMapper.MapToListModel(entity);
        var returnedModels = await _facadeSUT.GetAsync();
        var returnedModel = returnedModels.Single(i => i.Id == ActivitySeeds.ActivityEntity1.Id);

        //Assert
        Assert.Equal(check, returnedModel);
    }

    [Fact]
    public async Task GetFiltered_FromSeeded_ContainsSeeded()
    {
        //Arrange & Act
        var entity = ActivitySeeds.ActivityEntity1;
        var check = ActivityModelMapper.MapToListModel(entity);
        var returnedModels = await _facadeSUT.GetAsync("IAL", null, false);
        var returnedModel = returnedModels.First();

        //Assert
        Assert.Equal(check, returnedModel);
    }


    [Fact]
    public async Task Update_FromSeeded_DoesNotThrow()
    {
        //Arrange
        var detailModel = ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1);
        detailModel.Description = "Changed recipe name";

        //Act & Assert
        await _facadeSUT.SaveAsync(detailModel with {Students = default!});
    }

    [Fact]
    public async Task Update_Name_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1);
        detailModel.Description = "Changed activity description";
        detailModel.Type = ActivityType.Consultation;

        //Act
        await _facadeSUT.SaveAsync(detailModel with {Students = default!, Subject = default!});

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Update_RemoveRatings_FromSeeded_NotUpdated()
    {
        //Arrange
        var detailModel = ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1);
        detailModel.Students.Clear();

        //Act
        await _facadeSUT.SaveAsync(detailModel);

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1), returnedModel);
    }

    [Fact]
    public async Task Update_RemoveOneOfRatings_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1);
        detailModel.Students.Remove(detailModel.Students.First());

        //Act
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>  _facadeSUT.SaveAsync(detailModel));

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(ActivityModelMapper.MapToDetailModel(ActivitySeeds.ActivityEntity1), returnedModel);
    }

    [Fact]
    public async Task DeleteById_FromSeeded_DoesNotThrow()
    {
        //Arrange & Act & Assert
        await _facadeSUT.DeleteAsync(ActivitySeeds.ActivityEntity1.Id);
    }

    [Fact]
    public async Task DeleteNumOfActivitiesDecreased()
    {
        //Arrange
        var originalList = await _facadeSUT.GetAsync();
        int originalCount = originalList.Count();

        //Act
        await _facadeSUT.DeleteAsync(ActivitySeeds.ActivityEntity1.Id);

        //Assert
        var newList = await _facadeSUT.GetAsync();
        int newCount = newList.Count();
        Assert.Equal(newCount, originalCount - 1);
    }

    private static void FixIds(ActivityDetailModel expectedModel, ActivityDetailModel returnedModel)
    {
        returnedModel.Id = expectedModel.Id;

        foreach (var ratingModel in returnedModel.Students)
        {
            var ratingDetailModel = expectedModel.Students.FirstOrDefault(i =>
                i.StudentName == ratingModel.StudentName
                && i.StudentSurname == ratingModel.StudentSurname
                && Math.Abs(i.Points - ratingModel.Points) <= 0
                && i.Note == ratingModel.Note);

            if (ratingDetailModel != null)
            {
                ratingModel.Id = ratingDetailModel.Id;
                ratingModel.StudentId = ratingDetailModel.StudentId;
            }
        }
    }
}
