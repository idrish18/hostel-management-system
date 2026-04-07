using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing room operations
/// </summary>
public interface IRoomService
{
    Task<RoomDto?> GetRoomByIdAsync(int roomId);
    Task<IEnumerable<RoomDto>> GetRoomsByHostelAsync(int hostelId);
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(int hostelId);
    Task<RoomDto> CreateRoomAsync(int hostelId, string roomNumber, int capacity);
    Task<bool> IsRoomFullAsync(int roomId);
    Task IncrementOccupancyAsync(int roomId);
    Task DecrementOccupancyAsync(int roomId);
    Task<RoomDto?> UpdateRoomAsync(int roomId, string roomNumber, int capacity);
    Task<bool> DeleteRoomAsync(int roomId);
}
