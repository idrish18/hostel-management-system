namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for cleaning record response
/// </summary>
public class CleaningRecordDto
{
    public int RecordId { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? Remarks { get; set; }
    public int? WorkerId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? CleanedAt { get; set; }
    public int DaysOverdue => Status == "Pending" ? (DateTime.Now - AssignedAt).Days : 0;
}
