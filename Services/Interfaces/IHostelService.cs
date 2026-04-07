using SmartHostelManagementSystem.Models.DTOs.Responses;

namespace SmartHostelManagementSystem.Services.Interfaces;

/// <summary>
/// Service interface for managing hostel operations
/// </summary>
public interface IHostelService
{
    Task<HostelDto?> GetHostelByIdAsync(int hostelId);
    Task<IEnumerable<HostelDto>> GetAllHostelsAsync();
    Task<HostelDto> CreateHostelAsync(string name, string location, string description, string phoneNumber, int totalRooms);
    Task<HostelDto?> UpdateHostelAsync(int hostelId, string name, string location, string description, string phoneNumber);
    Task<bool> DeleteHostelAsync(int hostelId);
}
