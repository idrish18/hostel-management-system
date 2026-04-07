namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for room response
/// </summary>
public class RoomDto
{
    public int RoomId { get; set; }
    public int HostelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int CurrentOccupancy { get; set; }
    public int AvailableSeats => Capacity - CurrentOccupancy;
    public bool IsFull => CurrentOccupancy >= Capacity;
    public bool IsEmpty => CurrentOccupancy == 0;
}
