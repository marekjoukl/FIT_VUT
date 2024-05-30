using InformationSystem.BL.Mappers.Interfaces;
using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Mappers;

public class StudentModelMapper(EnrollmentModelMapper enrollmentModelMapper) :
    ModelMapperBase<StudentEntity, StudentListModel, StudentDetailModel>,
    IStudentModelMapper
{
    public override StudentListModel MapToListModel(StudentEntity? entity)
        => entity is null
            ? StudentListModel.Empty
            : new StudentListModel {Id = entity.Id, Name = entity.Name, Surname = entity.Surname };

    public override StudentDetailModel MapToDetailModel(StudentEntity? entity)
        => entity is null
            ? StudentDetailModel.Empty
            : new StudentDetailModel
            {
                Id = entity.Id, Name = entity.Name, Surname = entity.Surname, PhotoUrl = entity.PhotoUrl,
                Subjects = enrollmentModelMapper.MapToListModel(entity.Subjects)
                    .ToObservableCollection()
            };

    public override StudentEntity MapToEntity(StudentDetailModel model)
        => new() { Id = model.Id, Name = model.Name, Surname = model.Surname, PhotoUrl = model.PhotoUrl};
}
