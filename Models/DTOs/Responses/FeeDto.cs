namespace SmartHostelManagementSystem.Models.DTOs.Responses;

/// <summary>
/// DTO for fee response
/// </summary>
public class FeeDto
{
    public int FeeId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal RemainingAmount => Amount - AmountPaid;
    public string Status { get; set; } = "Pending";
    public DateTime DueDate { get; set; }
    public bool IsOverdue => DateTime.Now > DueDate && RemainingAmount > 0;
    public string? ReceiptNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
