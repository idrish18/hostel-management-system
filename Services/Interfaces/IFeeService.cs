using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing fees and payments
/// </summary>
public interface IFeeService
{
    Task<FeeDto?> GetFeeByIdAsync(int feeId);
    Task<IEnumerable<FeeDto>> GetFeesByStudentAsync(int studentId);
    Task<IEnumerable<FeeDto>> GetFeesByHostelAsync(int hostelId);
    Task<IEnumerable<FeeDto>> GetPendingFeesAsync(int hostelId);
    Task<IEnumerable<FeeDto>> GetOverdueFeesAsync(int hostelId);
    Task<FeeDto> RecordFeeAsync(int studentId, decimal amount, string description, DateTime dueDate);
    Task RecordPaymentAsync(int feeId, decimal paymentAmount);
    Task<decimal> GetTotalPendingFeesAsync(int hostelId);
    Task<decimal> GetTotalFeesCollectedAsync(int hostelId);
    Task<int> GetStudentsWithPendingFeesCountAsync(int hostelId);
    Task<string> GenerateReceiptAsync(int feeId);
    Task<bool> DeleteFeeAsync(int feeId);
}
