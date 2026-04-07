using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing students
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all students in a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByHostel(int hostelId)
    {
        try
        {
            var students = await _studentService.GetStudentsByHostelAsync(hostelId);
            return Ok(new { success = true, data = students });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving students" });
        }
    }

    /// <summary>
    /// Get unassigned students in a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}/unassigned")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnassigned(int hostelId)
    {
        try
        {
            var students = await _studentService.GetUnassignedStudentsAsync(hostelId);
            return Ok(new { success = true, data = students });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unassigned students for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving unassigned students" });
        }
    }

    /// <summary>
    /// Get students in a room
    /// </summary>
    [HttpGet("room/{roomId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByRoom(int roomId)
    {
        try
        {
            var students = await _studentService.GetStudentsByRoomAsync(roomId);
            return Ok(new { success = true, data = students });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students for room {RoomId}", roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving students" });
        }
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { success = false, message = "Student not found" });

            return Ok(new { success = true, data = student });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student {StudentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving student" });
        }
    }

    /// <summary>
    /// Assign a student to a room
    /// </summary>
    [HttpPost("{studentId}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignToRoom(int studentId, [FromBody] AssignStudentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            await _studentService.AssignStudentToRoomAsync(studentId, request.RoomId);
            var student = await _studentService.GetStudentByIdAsync(studentId);
            return Ok(new { success = true, message = "Student assigned to room successfully", data = student });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning student {StudentId} to room {RoomId}", studentId, request.RoomId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error assigning student to room" });
        }
    }

    /// <summary>
    /// Unassign a student from their room
    /// </summary>
    [HttpDelete("{studentId}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnassignFromRoom(int studentId)
    {
        try
        {
            await _studentService.UnassignStudentAsync(studentId);
            return Ok(new { success = true, message = "Student unassigned from room successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error unassigning student" });
        }
    }
}
