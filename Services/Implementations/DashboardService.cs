using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IDashboardService for analytics and metrics
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly IComplaintService _complaintService;
    private readonly ICleaningService _cleaningService;
    private readonly IFeeService _feeService;
    private readonly IRoomService _roomService;
    private const string DASHBOARD_CACHE_KEY = "dashboard_";

    public DashboardService(
        ApplicationDbContext context,
        ICacheService cacheService,
        IComplaintService complaintService,
        ICleaningService cleaningService,
        IFeeService feeService,
        IRoomService roomService)
    {
        _context = context;
        _cacheService = cacheService;
        _complaintService = complaintService;
        _cleaningService = cleaningService;
        _feeService = feeService;
        _roomService = roomService;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int hostelId)
    {
        var cacheKey = $"{DASHBOARD_CACHE_KEY}{hostelId}";
        var cached = await _cacheService.GetAsync<DashboardSummaryDto>(cacheKey);
        if (cached != null) return cached;

        var summary = new DashboardSummaryDto();

        // Calculate metrics
        summary.TotalStudents = await _context.Students.CountAsync(s => s.HostelId == hostelId && !s.IsDeleted);
        summary.TotalRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostelId && !r.IsDeleted);
        summary.OccupiedRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostelId && r.CurrentOccupancy > 0 && !r.IsDeleted);
        summary.OccupancyPercentage = summary.TotalRooms > 0 
            ? (decimal)summary.OccupiedRooms / summary.TotalRooms * 100 
            : 0;

        summary.PendingComplaints = await _complaintService.GetPendingComplaintsCountAsync(hostelId);
        summary.PendingCleaningRooms = await _cleaningService.GetPendingCleaningCountAsync(hostelId);

        var today = DateTime.UtcNow.Date;
        summary.CleanedRoomsToday = await _context.CleaningRecords
            .CountAsync(c => c.Room.HostelId == hostelId && 
                            c.Date.Date == today && 
                            c.Status == "Cleaned" && 
                            !c.IsDeleted);

        summary.TotalPendingFees = await _feeService.GetTotalPendingFeesAsync(hostelId);
        summary.TotalFeesCollected = await _feeService.GetTotalFeesCollectedAsync(hostelId);
        summary.StudentsWithPendingFees = await _feeService.GetStudentsWithPendingFeesCountAsync(hostelId);

        summary.CriticalAlerts = await GetCriticalAlertsAsync(hostelId);

        await _cacheService.SetAsync(cacheKey, summary, TimeSpan.FromMinutes(5));
        return summary;
    }

    public async Task<IEnumerable<ComplaintDto>> GetRecentComplaintsAsync(int hostelId, int count = 10)
    {
        var complaints = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .Where(c => c.HostelId == hostelId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();

        return complaints.Select(c => new ComplaintDto
        {
            ComplaintId = c.ComplaintId,
            StudentId = c.StudentId,
            StudentName = c.Student?.User?.FullName ?? "Unknown",
            Title = c.Title,
            Description = c.Description,
            Status = c.Status,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<List<string>> GetCriticalAlertsAsync(int hostelId)
    {
        var alerts = new List<string>();

        // Check for over-capacity rooms
        var overcapacityRooms = await _context.Rooms
            .CountAsync(r => r.HostelId == hostelId && r.CurrentOccupancy > r.Capacity && !r.IsDeleted);
        if (overcapacityRooms > 0)
            alerts.Add($"⚠️ {overcapacityRooms} room(s) exceeded capacity");

        // Check for overdue fees
        var overdueCount = await _context.Fees
            .CountAsync(f => f.HostelId == hostelId && 
                            f.DueDate < DateTime.UtcNow && 
                            f.PaymentStatus != "Paid" && 
                            !f.IsDeleted);
        if (overdueCount > 0)
            alerts.Add($"💰 {overdueCount} overdue fee(s)");

        // Check for pending cleaning rooms
        var pendingCleaning = await _cleaningService.GetPendingCleaningCountAsync(hostelId);
        if (pendingCleaning > 0)
            alerts.Add($"🧹 {pendingCleaning} room(s) pending cleaning");

        return alerts;
    }

    public async Task<Dictionary<string, object>> GetUtilizationMetricsAsync(int hostelId)
    {
        var totalRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostelId && !r.IsDeleted);
        var occupiedRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostelId && r.CurrentOccupancy > 0 && !r.IsDeleted);
        var totalCapacity = await _context.Rooms
            .Where(r => r.HostelId == hostelId && !r.IsDeleted)
            .SumAsync(r => r.Capacity);
        var currentOccupancy = await _context.Rooms
            .Where(r => r.HostelId == hostelId && !r.IsDeleted)
            .SumAsync(r => r.CurrentOccupancy);

        return new Dictionary<string, object>
        {
            { "TotalRooms", totalRooms },
            { "OccupiedRooms", occupiedRooms },
            { "EmptyRooms", totalRooms - occupiedRooms },
            { "TotalCapacity", totalCapacity },
            { "CurrentOccupancy", currentOccupancy },
            { "UtilizationPercentage", totalCapacity > 0 ? (decimal)currentOccupancy / totalCapacity * 100 : 0 }
        };
    }

    public async Task<Dictionary<string, object>> GetComplaintMetricsAsync(int hostelId)
    {
        var pending = await _context.Complaints
            .CountAsync(c => c.HostelId == hostelId && c.Status == "Pending" && !c.IsDeleted);
        var inProgress = await _context.Complaints
            .CountAsync(c => c.HostelId == hostelId && c.Status == "In Progress" && !c.IsDeleted);
        var resolved = await _context.Complaints
            .CountAsync(c => c.HostelId == hostelId && c.Status == "Resolved" && !c.IsDeleted);
        var total = pending + inProgress + resolved;

        return new Dictionary<string, object>
        {
            { "TotalComplaints", total },
            { "PendingComplaints", pending },
            { "InProgressComplaints", inProgress },
            { "ResolvedComplaints", resolved },
            { "ResolutionRate", total > 0 ? (decimal)resolved / total * 100 : 0 }
        };
    }

    public async Task<Dictionary<string, object>> GetFeeMetricsAsync(int hostelId)
    {
        var totalFees = await _context.Fees
            .Where(f => f.HostelId == hostelId && !f.IsDeleted)
            .SumAsync(f => f.Amount);
        var collectedFees = await _context.Fees
            .Where(f => f.HostelId == hostelId && !f.IsDeleted)
            .SumAsync(f => f.AmountPaid);
        var pendingFees = totalFees - collectedFees;
        var overdueCount = await _context.Fees
            .CountAsync(f => f.HostelId == hostelId && 
                            f.DueDate < DateTime.UtcNow && 
                            f.PaymentStatus != "Paid" && 
                            !f.IsDeleted);

        return new Dictionary<string, object>
        {
            { "TotalFees", totalFees },
            { "CollectedFees", collectedFees },
            { "PendingFees", pendingFees },
            { "CollectionPercentage", totalFees > 0 ? (decimal)collectedFees / totalFees * 100 : 0 },
            { "OverdueFees", overdueCount }
        };
    }
}
