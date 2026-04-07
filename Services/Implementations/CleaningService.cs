using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of ICleaningService for cleaning management (CORE MODULE)
/// </summary>
public class CleaningService : ICleaningService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private const string TODAY_CLEANING_KEY = "today_cleaning_";
    private const string PENDING_CLEANING_KEY = "pending_cleaning_";

    public CleaningService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<CleaningRecordDto?> GetCleaningRecordByIdAsync(int recordId)
    {
        var record = await _context.CleaningRecords
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.RecordId == recordId && !c.IsDeleted);
        return record == null ? null : MapToDto(record);
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetCleaningRecordsByRoomAsync(int roomId)
    {
        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.RoomId == roomId && !c.IsDeleted)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
        return records.Select(MapToDto);
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetTodaysCleaningTasksAsync(int hostelId)
    {
        var cacheKey = $"{TODAY_CLEANING_KEY}{hostelId}:{DateTime.UtcNow:yyyyMMdd}";
        var cached = await _cacheService.GetAsync<List<CleaningRecordDto>>(cacheKey);
        if (cached != null) return cached;

        var today = DateTime.UtcNow.Date;
        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.Room.HostelId == hostelId && 
                       c.Date.Date == today && 
                       !c.IsDeleted)
            .OrderBy(c => c.Status == "Pending" ? 0 : 1)
            .ToListAsync();

        var dtos = records.Select(MapToDto).ToList();
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromHours(1));
        return dtos;
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetPendingCleaningRecordsAsync(int hostelId)
    {
        var cacheKey = $"{PENDING_CLEANING_KEY}{hostelId}";
        var cached = await _cacheService.GetAsync<List<CleaningRecordDto>>(cacheKey);
        if (cached != null) return cached;

        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.Room.HostelId == hostelId && 
                       c.Status == "Pending" && 
                       !c.IsDeleted)
            .OrderBy(c => c.Date)
            .ToListAsync();

        var dtos = records.Select(MapToDto).ToList();
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15));
        return dtos;
    }

    public async Task<int> GetPendingCleaningCountAsync(int hostelId)
    {
        return await _context.CleaningRecords
            .Where(c => c.Room.HostelId == hostelId && c.Status == "Pending" && !c.IsDeleted)
            .CountAsync();
    }

    public async Task<CleaningRecordDto> CreateCleaningRecordAsync(int roomId, int workerId)
    {
        var record = new CleaningRecord
        {
            RoomId = roomId,
            WorkerId = workerId,
            Status = "Pending",
            Date = DateTime.UtcNow
        };

        _context.CleaningRecords.Add(record);
        await _context.SaveChangesAsync();

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
        return new CleaningRecordDto
        {
            RecordId = record.RecordId,
            RoomId = record.RoomId,
            RoomNumber = room?.RoomNumber ?? "Unknown",
            Status = record.Status,
            WorkerId = record.WorkerId,
            AssignedAt = record.Date
        };
    }

    public async Task<CleaningRecordDto?> UpdateCleaningStatusAsync(int recordId, string status, string? remarks, int? workerId)
    {
        var record = await _context.CleaningRecords
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.RecordId == recordId && !c.IsDeleted);
        if (record == null) return null;

        record.Status = status;
        record.Remarks = remarks;
        if (workerId.HasValue) record.WorkerId = workerId.Value;
        if (status == "Cleaned") record.CleanedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await InvalidateCleaningCacheAsync(record.Room.HostelId);

        return MapToDto(record);
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetCleaningHistoryAsync(int roomId, int days = 30)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);
        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.RoomId == roomId && c.Date >= fromDate && !c.IsDeleted)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
        return records.Select(MapToDto);
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetCleaningReportAsync(int hostelId, DateTime date)
    {
        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.Room.HostelId == hostelId && 
                       c.Date.Date == date.Date && 
                       !c.IsDeleted)
            .ToListAsync();
        return records.Select(MapToDto);
    }

    public async Task<IEnumerable<CleaningRecordDto>> GetUncleanRoomsAsync(int hostelId, int thresholdDays = 3)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-thresholdDays);
        var records = await _context.CleaningRecords
            .Include(c => c.Room)
            .Where(c => c.Room.HostelId == hostelId && 
                       (c.Status == "Pending" || (c.CleanedAt.HasValue && c.CleanedAt < thresholdDate)) &&
                       !c.IsDeleted)
            .OrderBy(c => c.Date)
            .ToListAsync();
        return records.Select(MapToDto);
    }

    public async Task<bool> DeleteCleaningRecordAsync(int recordId)
    {
        var record = await _context.CleaningRecords.FirstOrDefaultAsync(c => c.RecordId == recordId && !c.IsDeleted);
        if (record == null) return false;

        record.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private static CleaningRecordDto MapToDto(CleaningRecord record)
    {
        return new CleaningRecordDto
        {
            RecordId = record.RecordId,
            RoomId = record.RoomId,
            RoomNumber = record.Room?.RoomNumber ?? "Unknown",
            Status = record.Status,
            Remarks = record.Remarks,
            WorkerId = record.WorkerId,
            AssignedAt = record.Date,
            CleanedAt = record.CleanedAt
        };
    }

    private async Task InvalidateCleaningCacheAsync(int hostelId)
    {
        var todayKey = $"{TODAY_CLEANING_KEY}{hostelId}:{DateTime.UtcNow:yyyyMMdd}";
        var pendingKey = $"{PENDING_CLEANING_KEY}{hostelId}";
        await _cacheService.RemoveAsync(todayKey);
        await _cacheService.RemoveAsync(pendingKey);
    }
}
