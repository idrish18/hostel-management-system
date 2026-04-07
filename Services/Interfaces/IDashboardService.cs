using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing dashboard analytics and metrics
/// </summary>
public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int hostelId);
    Task<IEnumerable<ComplaintDto>> GetRecentComplaintsAsync(int hostelId, int count = 10);
    Task<List<string>> GetCriticalAlertsAsync(int hostelId);
    Task<Dictionary<string, object>> GetUtilizationMetricsAsync(int hostelId);
    Task<Dictionary<string, object>> GetComplaintMetricsAsync(int hostelId);
    Task<Dictionary<string, object>> GetFeeMetricsAsync(int hostelId);
}
