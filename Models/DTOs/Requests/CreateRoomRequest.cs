using System.ComponentModel.DataAnnotations;

namespace SmartHostelManagementSystem.Models.DTOs.Requests;

public class CreateRoomRequest
{
    [Required]
    public int HostelId { get; set; }

    [Required]
    [StringLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 20)]
    public int Capacity { get; set; }
}
