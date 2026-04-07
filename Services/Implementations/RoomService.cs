using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IRoomService for room management
/// </summary>
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private const string ROOM_CACHE_KEY = "room_";
    private const string AVAILABLE_ROOMS_KEY = "available_rooms_";

    public RoomService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int roomId)
    {
        var cacheKey = $"{ROOM_CACHE_KEY}{roomId}";
        var cached = await _cacheService.GetAsync<RoomDto>(cacheKey);
        if (cached != null) return cached;

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room == null) return null;

        var dto = MapToDto(room);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsByHostelAsync(int hostelId)
    {
        var rooms = await _context.Rooms
            .Where(r => r.HostelId == hostelId && !r.IsDeleted)
            .ToListAsync();
        return rooms.Select(MapToDto);
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(int hostelId)
    {
        var cacheKey = $"{AVAILABLE_ROOMS_KEY}{hostelId}";
        var cached = await _cacheService.GetAsync<List<RoomDto>>(cacheKey);
        if (cached != null) return cached;

        var rooms = await _context.Rooms
            .Where(r => r.HostelId == hostelId && !r.IsDeleted && r.CurrentOccupancy < r.Capacity)
            .ToListAsync();

        var dtos = rooms.Select(MapToDto).ToList();
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(15));
        return dtos;
    }

    public async Task<RoomDto> CreateRoomAsync(int hostelId, string roomNumber, int capacity)
    {
        var room = new Room
        {
            HostelId = hostelId,
            RoomNumber = roomNumber,
            Capacity = capacity,
            CurrentOccupancy = 0
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return MapToDto(room);
    }

    public async Task<bool> IsRoomFullAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        return room?.CurrentOccupancy >= room?.Capacity;
    }

    public async Task IncrementOccupancyAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room != null && room.CurrentOccupancy < room.Capacity)
        {
            room.CurrentOccupancy++;
            await _context.SaveChangesAsync();
            await InvalidateRoomCacheAsync(roomId);
        }
    }

    public async Task DecrementOccupancyAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room != null && room.CurrentOccupancy > 0)
        {
            room.CurrentOccupancy--;
            await _context.SaveChangesAsync();
            await InvalidateRoomCacheAsync(roomId);
        }
    }

    public async Task<RoomDto?> UpdateRoomAsync(int roomId, string roomNumber, int capacity)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room == null) return null;

        room.RoomNumber = roomNumber;
        room.Capacity = capacity;
        await _context.SaveChangesAsync();
        await InvalidateRoomCacheAsync(roomId);

        return MapToDto(room);
    }

    public async Task<bool> DeleteRoomAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room == null) return false;

        room.IsDeleted = true;
        await _context.SaveChangesAsync();
        await InvalidateRoomCacheAsync(roomId);

        return true;
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
        {
            RoomId = room.RoomId,
            HostelId = room.HostelId,
            RoomNumber = room.RoomNumber,
            Capacity = room.Capacity,
            CurrentOccupancy = room.CurrentOccupancy
        };
    }

    private async Task InvalidateRoomCacheAsync(int roomId)
    {
        var cacheKey = $"{ROOM_CACHE_KEY}{roomId}";
        await _cacheService.RemoveAsync(cacheKey);
    }
}
