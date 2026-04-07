using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing student assignments and data
/// </summary>
public interface IStudentService
{
    Task<StudentDto?> GetStudentByIdAsync(int studentId);
    Task<IEnumerable<StudentDto>> GetStudentsByHostelAsync(int hostelId);
    Task<IEnumerable<StudentDto>> GetStudentsByRoomAsync(int roomId);
    Task<IEnumerable<StudentDto>> GetUnassignedStudentsAsync(int hostelId);
    Task AssignStudentToRoomAsync(int studentId, int roomId);
    Task UnassignStudentAsync(int studentId);
    Task<int> GetStudentCountAsync(int hostelId);
}
