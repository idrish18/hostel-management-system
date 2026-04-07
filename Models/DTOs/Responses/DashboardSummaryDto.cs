namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for dashboard summary response
/// </summary>
public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public decimal OccupancyPercentage { get; set; }
    public int PendingComplaints { get; set; }
    public int PendingCleaningRooms { get; set; }
    public int CleanedRoomsToday { get; set; }
    public decimal TotalPendingFees { get; set; }
    public decimal TotalFeesCollected { get; set; }
    public int StudentsWithPendingFees { get; set; }
    public List<string> CriticalAlerts { get; set; } = new();
}
