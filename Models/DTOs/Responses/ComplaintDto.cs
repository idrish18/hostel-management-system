namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for complaint response
/// </summary>
public class ComplaintDto
{
    public int ComplaintId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int DaysOpen => (DateTime.Now - CreatedAt).Days;
}
