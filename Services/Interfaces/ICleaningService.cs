using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing cleaning records and tasks (CORE MODULE)
/// </summary>
public interface ICleaningService
{
    Task<CleaningRecordDto?> GetCleaningRecordByIdAsync(int recordId);
    Task<IEnumerable<CleaningRecordDto>> GetCleaningRecordsByRoomAsync(int roomId);
    Task<IEnumerable<CleaningRecordDto>> GetTodaysCleaningTasksAsync(int hostelId);
    Task<IEnumerable<CleaningRecordDto>> GetPendingCleaningRecordsAsync(int hostelId);
    Task<int> GetPendingCleaningCountAsync(int hostelId);
    Task<CleaningRecordDto> CreateCleaningRecordAsync(int roomId, int workerId);
    Task<CleaningRecordDto?> UpdateCleaningStatusAsync(int recordId, string status, string? remarks, int? workerId);
    Task<IEnumerable<CleaningRecordDto>> GetCleaningHistoryAsync(int roomId, int days = 30);
    Task<IEnumerable<CleaningRecordDto>> GetCleaningReportAsync(int hostelId, DateTime date);
    Task<IEnumerable<CleaningRecordDto>> GetUncleanRoomsAsync(int hostelId, int thresholdDays = 3);
    Task<bool> DeleteCleaningRecordAsync(int recordId);
}
