using InformationSystem.BL.Facades.Interfaces;
using InformationSystem.BL.Facades;
using InformationSystem.BL.Models;
using InformationSystem.Common.Tests;
using InformationSystem.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace InformationSystem.BL.Tests;

public sealed class StudentFacadeTests : FacadeTestsBase
{
    private readonly IStudentFacade _studentFacadeSUT;

    public StudentFacadeTests(ITestOutputHelper output) : base(output)
    {
        _studentFacadeSUT = new StudentFacade(UnitOfWorkFactory, StudentModelMapper);
    }

    [Fact]
    public async Task Create_WithNonExistingItem_DoesNotThrow()
    {
        var model = new StudentDetailModel()
        {
            Id = Guid.Empty, Name = "Franz", Surname = "Ferdinand", PhotoUrl = null
        };

        var _ = await _studentFacadeSUT.SaveAsync(model);
    }

    [Fact]
    public async Task GetAll_Single_SeededStudent()
    {
        var students = await _studentFacadeSUT.GetAsync();
        var student = students.Single(i => i.Id == StudentSeeds.StudentEntity0.Id);

        DeepAssert.Equal(StudentModelMapper.MapToListModel(StudentSeeds.StudentEntity0), student);
    }

    [Fact]
    public async Task GetById_SeededStudent()
    {
        var detailModel = StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1);

        var returnModel = await _studentFacadeSUT.GetAsync(StudentSeeds.StudentEntity1.Id);

        DeepAssert.Equal(detailModel, returnModel);
    }

    [Fact]
    public async Task GetById_NonExistent()
    {
        var ingredient = await _studentFacadeSUT.GetAsync(StudentSeeds.EmptyStudent.Id);

        Assert.Null(ingredient);
    }

    [Fact]
    public async Task GetByName()
    {
        //Arrange
        var listModel = StudentModelMapper.MapToListModel(StudentSeeds.StudentEntity1);

        //Act
        var returnedModels =
            await _studentFacadeSUT.GetByNameOrSurnameAsync(listModel.Surname);
        var returnedModel = returnedModels.First();

        //Assert
        DeepAssert.Equal(listModel, returnedModel, ["Subjects"]);
    }

    [Fact]
    public async Task SortByName_Asc()
    {
        //Arrange
        var listModel = StudentModelMapper.MapToListModel(StudentSeeds.StudentEntity0);

        //Act
        var students = await _studentFacadeSUT.GetAsync("Surname", false);

        //Assert
        var student = students.First();
        DeepAssert.Equal(listModel, student);
    }

    [Fact]
    public async Task SortByNameDesc()
    {
        //Arrange
        var listModel = StudentModelMapper.MapToListModel(StudentSeeds.StudentEntity1);

        //Act
        var students = await _studentFacadeSUT.GetAsync("Surname", true);

        //Assert
        var student = students.First();
        DeepAssert.Equal(listModel, student);
    }

    [Fact]
    public async Task SeededStudent_DeleteById_Deleted()
    {
        await _studentFacadeSUT.DeleteAsync(StudentSeeds.StudentEntity0.Id);

        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await dbxAssert.Students.AnyAsync(i => i.Id == StudentSeeds.StudentEntity0.Id));
    }

    [Fact]
    public async Task Update_Name_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1);
        detailModel.Name = "Changed recipe name 1";

        //Act
        await _studentFacadeSUT.SaveAsync(detailModel with { Subjects = default! });

        //Assert
        var returnedModel = await _studentFacadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task NewStudent_InsertOrUpdate_StudentAdded()
    {
        //Arrange
        var student = new StudentDetailModel()
        {
            Id = Guid.Empty, Name = "Fred", Surname = "Fredington", PhotoUrl = null
        };

        //Act
        student = await _studentFacadeSUT.SaveAsync(student);

        //Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var studentFromDb = await dbxAssert.Students.SingleAsync(i => i.Id == student.Id);
        DeepAssert.Equal(student, StudentModelMapper.MapToDetailModel(studentFromDb));
    }

    [Fact]
    public async Task SeededStudent_InsertOrUpdate_StudentUpdated()
    {
        //Arrange
        var student = new StudentDetailModel()
        {
            Id = StudentSeeds.StudentEntity0.Id, Name = StudentSeeds.StudentEntity0.Name, Surname = StudentSeeds.StudentEntity0.Surname,
        };
        student.Name += "updated";
        student.Surname += "updated";

        //Act
        await _studentFacadeSUT.SaveAsync(student);

        //Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var studentFromDb = await dbxAssert.Students.SingleAsync(i => i.Id == student.Id);
        DeepAssert.Equal(student, StudentModelMapper.MapToDetailModel(studentFromDb));
    }
    [Fact]
    public async Task Update_RemoveSubjects_FromSeeded_NotUpdated()
    {
        //Arrange
        var detailModel = StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1);
        detailModel.Subjects.Clear();

        //Act
        await _studentFacadeSUT.SaveAsync(detailModel);

        //Assert
        var returnedModel = await _studentFacadeSUT.GetAsync(detailModel.Id);
        var compereModel = StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1);
        DeepAssert.Equal(compereModel, returnedModel);
    }

    // [Fact]
    // public async Task Update_RemoveOneOfIngredients_FromSeeded_Updated()
    // {
    //     //Arrange
    //     var detailModel = StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1);
    //     detailModel.Subjects.Remove(detailModel.Subjects.First());
    //
    //     //Act
    //     await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>  _studentFacadeSUT.SaveAsync(detailModel));
    //
    //     //Assert
    //     var returnedModel = await _studentFacadeSUT.GetAsync(detailModel.Id);
    //     DeepAssert.Equal(StudentModelMapper.MapToDetailModel(StudentSeeds.StudentEntity1), returnedModel);
    // }

}
