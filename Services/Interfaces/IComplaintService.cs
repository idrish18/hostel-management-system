using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing complaints
/// </summary>
public interface IComplaintService
{
    Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId);
    Task<IEnumerable<ComplaintDto>> GetComplaintsByHostelAsync(int hostelId);
    Task<IEnumerable<ComplaintDto>> GetComplaintsByStatusAsync(int hostelId, string status);
    Task<IEnumerable<ComplaintDto>> GetComplaintsByStudentAsync(int studentId);
    Task<ComplaintDto> RaiseComplaintAsync(int studentId, string title, string description);
    Task<ComplaintDto?> UpdateComplaintStatusAsync(int complaintId, string status, string? resolution);
    Task<int> GetPendingComplaintsCountAsync(int hostelId);
    Task<bool> DeleteComplaintAsync(int complaintId);
}
