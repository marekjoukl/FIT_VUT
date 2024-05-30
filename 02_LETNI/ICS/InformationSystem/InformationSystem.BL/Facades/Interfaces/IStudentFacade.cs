using InformationSystem.BL.Models;
using InformationSystem.DAL.Entities;

namespace InformationSystem.BL.Facades.Interfaces;

public interface IStudentFacade : IFacade<StudentEntity,StudentListModel, StudentDetailModel>
{
    Task<IEnumerable<StudentListModel>> GetByNameOrSurnameAsync(string searchedName);
    Task<IEnumerable<StudentListModel>> GetByFullNameAsync(string fullName);
}
