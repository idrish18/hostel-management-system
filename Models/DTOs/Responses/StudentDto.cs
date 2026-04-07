namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for student response
/// </summary>
public class StudentDto
{
    public int StudentId { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAssignedToRoom { get; set; }
    public int? RoomId { get; set; }
    public string? RoomNumber { get; set; }
}
