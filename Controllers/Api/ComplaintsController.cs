using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing complaints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;
    private readonly ILogger<ComplaintsController> _logger;

    public ComplaintsController(IComplaintService complaintService, ILogger<ComplaintsController> logger)
    {
        _complaintService = complaintService;
        _logger = logger;
    }

    /// <summary>
    /// Get all complaints for a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByHostel(int hostelId)
    {
        try
        {
            var complaints = await _complaintService.GetComplaintsByHostelAsync(hostelId);
            return Ok(new { success = true, data = complaints });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaints for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving complaints" });
        }
    }

    /// <summary>
    /// Get complaints filtered by status
    /// </summary>
    [HttpGet("hostel/{hostelId}/status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByStatus(int hostelId, string status)
    {
        try
        {
            var complaints = await _complaintService.GetComplaintsByStatusAsync(hostelId, status);
            return Ok(new { success = true, data = complaints });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaints by status for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving complaints" });
        }
    }

    /// <summary>
    /// Get complaints filed by a student
    /// </summary>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        try
        {
            var complaints = await _complaintService.GetComplaintsByStudentAsync(studentId);
            return Ok(new { success = true, data = complaints });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaints for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving complaints" });
        }
    }

    /// <summary>
    /// Get complaint by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var complaint = await _complaintService.GetComplaintByIdAsync(id);
            if (complaint == null)
                return NotFound(new { success = false, message = "Complaint not found" });

            return Ok(new { success = true, data = complaint });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaint {ComplaintId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving complaint" });
        }
    }

    /// <summary>
    /// Raise a new complaint
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] RaiseComplaintRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var complaint = await _complaintService.RaiseComplaintAsync(
                request.StudentId,
                request.Title,
                request.Description);

            return CreatedAtAction(nameof(GetById), new { id = complaint.ComplaintId }, 
                new { success = true, data = complaint });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating complaint");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error creating complaint" });
        }
    }

    /// <summary>
    /// Update complaint status
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var complaint = await _complaintService.UpdateComplaintStatusAsync(id, request.Status, request.Resolution);
            if (complaint == null)
                return NotFound(new { success = false, message = "Complaint not found" });

            return Ok(new { success = true, data = complaint });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating complaint {ComplaintId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error updating complaint" });
        }
    }

    /// <summary>
    /// Delete a complaint
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _complaintService.DeleteComplaintAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Complaint not found" });

            return Ok(new { success = true, message = "Complaint deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting complaint {ComplaintId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error deleting complaint" });
        }
    }
}
