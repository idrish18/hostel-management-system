using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IComplaintService for complaint management
/// </summary>
public class ComplaintService : IComplaintService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public ComplaintService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId)
    {
        var complaint = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(c => c.ComplaintId == complaintId && !c.IsDeleted);
        return complaint == null ? null : MapToDto(complaint);
    }

    public async Task<IEnumerable<ComplaintDto>> GetComplaintsByHostelAsync(int hostelId)
    {
        var complaints = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .Where(c => c.HostelId == hostelId && !c.IsDeleted)
            .ToListAsync();
        return complaints.Select(MapToDto);
    }

    public async Task<IEnumerable<ComplaintDto>> GetComplaintsByStatusAsync(int hostelId, string status)
    {
        var complaints = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .Where(c => c.HostelId == hostelId && c.Status == status && !c.IsDeleted)
            .ToListAsync();
        return complaints.Select(MapToDto);
    }

    public async Task<IEnumerable<ComplaintDto>> GetComplaintsByStudentAsync(int studentId)
    {
        var complaints = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .Where(c => c.StudentId == studentId && !c.IsDeleted)
            .ToListAsync();
        return complaints.Select(MapToDto);
    }

    public async Task<ComplaintDto> RaiseComplaintAsync(int studentId, string title, string description)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        var complaint = new Complaint
        {
            StudentId = studentId,
            HostelId = student?.HostelId ?? 0,
            Title = title,
            Description = description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync();

        return new ComplaintDto
        {
            ComplaintId = complaint.ComplaintId,
            StudentId = complaint.StudentId,
            StudentName = student?.User?.FullName ?? "Unknown",
            Title = complaint.Title,
            Description = complaint.Description,
            Status = complaint.Status,
            CreatedAt = complaint.CreatedAt
        };
    }

    public async Task<ComplaintDto?> UpdateComplaintStatusAsync(int complaintId, string status, string? resolution)
    {
        var complaint = await _context.Complaints
            .Include(c => c.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(c => c.ComplaintId == complaintId && !c.IsDeleted);
        if (complaint == null) return null;

        complaint.Status = status;
        complaint.Resolution = resolution;
        if (status == "Resolved")
            complaint.ResolvedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(complaint);
    }

    public async Task<int> GetPendingComplaintsCountAsync(int hostelId)
    {
        return await _context.Complaints
            .Where(c => c.HostelId == hostelId && c.Status == "Pending" && !c.IsDeleted)
            .CountAsync();
    }

    public async Task<bool> DeleteComplaintAsync(int complaintId)
    {
        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId && !c.IsDeleted);
        if (complaint == null) return false;

        complaint.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private static ComplaintDto MapToDto(Complaint complaint)
    {
        return new ComplaintDto
        {
            ComplaintId = complaint.ComplaintId,
            StudentId = complaint.StudentId,
            StudentName = complaint.Student?.User?.FullName ?? "Unknown",
            Title = complaint.Title,
            Description = complaint.Description,
            Status = complaint.Status,
            Resolution = complaint.Resolution,
            CreatedAt = complaint.CreatedAt,
            ResolvedAt = complaint.ResolvedDate
        };
    }
}
