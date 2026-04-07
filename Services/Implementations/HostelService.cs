using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IHostelService for hostel management
/// </summary>
public class HostelService : IHostelService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private const string HOSTEL_CACHE_KEY = "hostel_";

    public HostelService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<HostelDto?> GetHostelByIdAsync(int hostelId)
    {
        var cacheKey = $"{HOSTEL_CACHE_KEY}{hostelId}";
        var cached = await _cacheService.GetAsync<HostelDto>(cacheKey);
        if (cached != null) return cached;

        var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.HostelId == hostelId && !h.IsDeleted);
        if (hostel == null) return null;

        var dto = MapToDto(hostel);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromHours(1));
        return dto;
    }

    public async Task<IEnumerable<HostelDto>> GetAllHostelsAsync()
    {
        var hostels = await _context.Hostels
            .Where(h => !h.IsDeleted)
            .ToListAsync();
        return hostels.Select(MapToDto);
    }

    public async Task<HostelDto> CreateHostelAsync(string name, string location, string description, string phoneNumber, int totalRooms)
    {
        var hostel = new Hostel
        {
            Name = name,
            Location = location,
            Description = description,
            PhoneNumber = phoneNumber,
            Capacity = totalRooms,
            CreatedAt = DateTime.UtcNow
        };

        _context.Hostels.Add(hostel);
        await _context.SaveChangesAsync();

        return MapToDto(hostel);
    }

    public async Task<HostelDto?> UpdateHostelAsync(int hostelId, string name, string location, string description, string phoneNumber)
    {
        var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.HostelId == hostelId && !h.IsDeleted);
        if (hostel == null) return null;

        hostel.Name = name;
        hostel.Location = location;
        hostel.Description = description;
        hostel.PhoneNumber = phoneNumber;
        hostel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        var cacheKey = $"{HOSTEL_CACHE_KEY}{hostelId}";
        await _cacheService.RemoveAsync(cacheKey);

        return MapToDto(hostel);
    }

    public async Task<bool> DeleteHostelAsync(int hostelId)
    {
        var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.HostelId == hostelId && !h.IsDeleted);
        if (hostel == null) return false;

        hostel.IsDeleted = true;
        hostel.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var cacheKey = $"{HOSTEL_CACHE_KEY}{hostelId}";
        await _cacheService.RemoveAsync(cacheKey);

        return true;
    }

    private static HostelDto MapToDto(Hostel hostel)
    {
        return new HostelDto
        {
            HostelId = hostel.HostelId,
            Name = hostel.Name,
            Location = hostel.Location,
            Description = hostel.Description ?? string.Empty,
            PhoneNumber = hostel.PhoneNumber ?? string.Empty,
            TotalRooms = hostel.Capacity,
            OccupiedRooms = 0, // Will be calculated from rooms
            TotalStudents = 0, // Will be calculated from students
            CreatedAt = hostel.CreatedAt,
            UpdatedAt = hostel.UpdatedAt
        };
    }
}
