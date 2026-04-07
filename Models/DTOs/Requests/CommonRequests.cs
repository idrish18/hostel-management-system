using System.ComponentModel.DataAnnotations;

namespace SmartHostelManagementSystem.Models.DTOs.Requests;

public class AssignStudentRequest
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int RoomId { get; set; }
}

public class RaiseComplaintRequest
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;
}

public class RecordFeeRequest
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }
}

public class UpdateComplaintStatusRequest
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Resolution { get; set; }
}

public class UpdateCleaningStatusRequest
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Remarks { get; set; }

    public int? WorkerId { get; set; }
}

public class RecordPaymentRequest
{
    [Required]
    [Range(0.01, 1000000)]
    public decimal PaymentAmount { get; set; }
}
