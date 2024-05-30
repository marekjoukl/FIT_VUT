using System.Collections.ObjectModel;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.Common.Enums;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace InformationSystem.BL.Tests;

public sealed class SubjectFacadeTests : FacadeTestsBase
{
    private readonly ISubjectFacade _facadeSUT;

    public SubjectFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new SubjectFacade(UnitOfWorkFactory, SubjectModelModelMapper);
    }

    // [Fact]
    // public async Task Create_WithWithoutActivity_EqualsCreated()
    // {
    //     //Arrange
    //     var model = new SubjectDetailModel()
    //     {
    //         Name = "Matematika",
    //         Abbreviation = "M",
    //     };
    //
    //     //Act
    //     var returnedModel = await _facadeSUT.SaveAsync(model);
    //
    //     //Assert
    //     FixIds(model, returnedModel);
    //     DeepAssert.Equal(model, returnedModel);
    // }

    // [Fact]
    // public async Task Create_WithNonExistingActivity_Throws()
    // {
    //     //Arrange
    //     var model = new SubjectDetailModel()
    //     {
    //         Name = "Dejepis",
    //         Abbreviation = "D",
    //         Activities = new ObservableCollection<ActivityListModel>()
    //         {
    //             new()
    //             {
    //                 Id = Guid.Empty,
    //                 Start = DateTime.Now,
    //                 End = DateTime.Now.AddHours(1),
    //                 Room = ActivityRoom.A5,
    //                 Type = ActivityType.Seminar,
    //                 SubjectId = Guid.Empty,
    //                 SubjectName = ""
    //             }
    //         }
    //     };
    //
    //     //Act & Assert
    //     await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    // }

    // [Fact]
    // public async Task Create_WithExistingActivity_Throws()
    // {
    //     //Arrange
    //     var model = new SubjectDetailModel()
    //     {
    //         Name = "Geografia",
    //         Abbreviation = "G",
    //         Activities = new ObservableCollection<ActivityListModel>()
    //         {
    //             new ()
    //             {
    //                 Start = DateTime.Now,
    //                 End = DateTime.Now.AddHours(1),
    //                 Room = ActivityRoom.A5,
    //                 Type = ActivityType.Seminar,
    //                 SubjectId = ActivitySeeds.ActivityEntity1.SubjectId,
    //                 SubjectName = ActivitySeeds.ActivityEntity1.Subject.Name
    //             }
    //         }
    //     };
    //
    //     //Act && Assert
    //     await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    // }

    // [Fact]
    // public async Task Create_WithExistingAndNotExistingActivity_Throws()
    // {
    //     //Arrange
    //     var model = new SubjectDetailModel()
    //     {
    //         Name = "Fyzika",
    //         Abbreviation = "F",
    //         Activities =
    //         [
    //             new()
    //             {
    //                 Start = DateTime.Now,
    //                 End = DateTime.Now.AddHours(1),
    //                 Room = ActivityRoom.A5,
    //                 Type = ActivityType.Seminar,
    //                 SubjectId = Guid.Empty,
    //                 SubjectName = ""
    //             },
    //
    //             ActivityModelMapper.MapToListModel(ActivitySeeds.ActivityEntity1)
    //
    //         ],
    //     };
    //
    //     //Act & Assert
    //     await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    // }

    [Fact]
    public async Task GetById_FromSeeded_EqualsSeeded()
    {
        //Arrange
        var detailModel = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity1);

        //Act
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        //Assert
        DeepAssert.Equal(detailModel, returnedModel, ["Activities","Students"]);
    }

    [Fact]
    public async Task GetByAbb_FromSeeded_EqualsSeeded()
    {
        //Arrange
        var listModel = SubjectModelModelMapper.MapToListModel(SubjectSeeds.SubjectEntity1);

        //Act
        var returnedModels = await _facadeSUT.FilterByAbbAsync(listModel.Abbreviation);
        var returnedModel = returnedModels.First();

        //Assert
        DeepAssert.Equal(listModel, returnedModel, ["Activities","Students"]);
    }

    [Fact]
    public async Task GetAll_FromSeeded_ContainsSeeded()
    {
        //Arrange
        var listModel = SubjectModelModelMapper.MapToListModel(SubjectSeeds.SubjectEntity2);

        //Act
        var returnedModel = await _facadeSUT.GetAsync();

        //Assert
        Assert.Contains(listModel, returnedModel);
    }

    [Fact]
    public async Task SortByAbbAsc()
    {
        //Arrange
        var listModel = SubjectModelModelMapper.MapToListModel(SubjectSeeds.SubjectEntity1);

        //Act
        var returnedModels = await _facadeSUT.GetAsync("Abbreviation", false);
        var returnedModel = returnedModels.First();

        //Assert
        Assert.Equal(listModel, returnedModel);
    }

    [Fact]
    public async Task SortByNameDesc()
    {
        //Arrange
        var listModel = SubjectModelModelMapper.MapToListModel(SubjectSeeds.SubjectEntity0);

        //Act
        var returnedModels = await _facadeSUT.GetAsync("Name", true);
        var returnedModel = returnedModels.First();

        //Assert
        Assert.Equal(listModel, returnedModel);
    }

    [Fact]
    public async Task Update_Name_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity1);
        detailModel.Name = "Changed recipe name 1";

        //Act
        await _facadeSUT.SaveAsync(detailModel);

        //Assert
        var entity = await InformationSystemDbContextSUT.Subjects.SingleAsync(i => i.Id == SubjectSeeds.SubjectEntity1.Id);
        var returnedModel = SubjectModelModelMapper.MapToDetailModel(entity);
        DeepAssert.Equal(detailModel, returnedModel, ["Activities", "Students"]);
    }

    // [Fact]
    // public async Task Update_RemoveActivities_FromSeeded_NotUpdated()
    // {
    //     //Arrange
    //     var detailModel = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity0);
    //     detailModel.Activities.Clear();
    //
    //     //Act
    //     await _facadeSUT.SaveAsync(detailModel);
    //
    //     //Assert
    //     var entity = await InformationSystemDbContextSUT.Subjects.SingleAsync(i => i.Id == SubjectSeeds.SubjectEntity0.Id);
    //     var returnedModel = SubjectModelModelMapper.MapToDetailModel(entity);
    //     DeepAssert.Equal(SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity0), returnedModel, ["Activities", "Students"]);
    // }

    // [Fact]
    // public async Task Update_RemoveOneOfActivities_FromSeeded_Updated()
    // {
    //     //Arrange
    //     var detailModel = SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity0);
    //     detailModel.Activities.Remove(detailModel.Activities.First());
    //
    //     //Act
    //     await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>  _facadeSUT.SaveAsync(detailModel));
    //
    //     //Assert
    //     var entity = await InformationSystemDbContextSUT.Subjects.SingleAsync(i => i.Id == SubjectSeeds.SubjectEntity0.Id);
    //     var returnedModel = SubjectModelModelMapper.MapToDetailModel(entity);
    //     DeepAssert.Equal(SubjectModelModelMapper.MapToDetailModel(SubjectSeeds.SubjectEntity0), returnedModel, ["Activities", "Students"]);
    // }

    [Fact]
    public async Task DeleteById_FromSeeded_DoesNotThrow()
    {
        //Arrange & Act & Assert
        await _facadeSUT.DeleteAsync(SubjectSeeds.SubjectEntity1.Id);
    }

    // private static void FixIds(SubjectDetailModel expectedModel, SubjectDetailModel returnedModel)
    // {
    //     returnedModel.Id = expectedModel.Id;
    //
    //     foreach (var activityModel in returnedModel.Activities)
    //     {
    //         var activityDetailModel = expectedModel.Activities.FirstOrDefault(i =>
    //             i.Start == activityModel.Start
    //             && i.End == activityModel.End
    //             && i.Room == activityModel.Room
    //             && i.Type == activityModel.Type
    //             && i.SubjectId == activityModel.SubjectId
    //             && i.SubjectName == activityModel.SubjectName);
    //
    //         if (activityDetailModel != null)
    //         {
    //             activityModel.Id = activityDetailModel.Id;
    //             activityModel.SubjectId = activityDetailModel.SubjectId;
    //         }
    //     }
    // }
}
