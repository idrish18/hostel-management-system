using System.ComponentModel.DataAnnotations;

namespace SmartHostelManagementSystem.Models.DTOs.Requests;

public class CreateHostelRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int TotalRooms { get; set; }
}
