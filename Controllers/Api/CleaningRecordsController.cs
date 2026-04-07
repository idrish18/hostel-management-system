using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing cleaning records and tasks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CleaningRecordsController : ControllerBase
{
    private readonly ICleaningService _cleaningService;
    private readonly ILogger<CleaningRecordsController> _logger;

    public CleaningRecordsController(ICleaningService cleaningService, ILogger<CleaningRecordsController> logger)
    {
        _cleaningService = cleaningService;
        _logger = logger;
    }

    /// <summary>
    /// Get cleaning records for a room
    /// </summary>
    [HttpGet("room/{roomId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByRoom(int roomId)
    {
        try
        {
            var records = await _cleaningService.GetCleaningRecordsByRoomAsync(roomId);
            return Ok(new { success = true, data = records });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cleaning records for room {RoomId}", roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving cleaning records" });
        }
    }

    /// <summary>
    /// Get today's cleaning tasks
    /// </summary>
    [HttpGet("hostel/{hostelId}/today")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTodaysTasks(int hostelId)
    {
        try
        {
            var tasks = await _cleaningService.GetTodaysCleaningTasksAsync(hostelId);
            return Ok(new { success = true, data = tasks });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's cleaning tasks for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving cleaning tasks" });
        }
    }

    /// <summary>
    /// Get pending cleaning tasks
    /// </summary>
    [HttpGet("hostel/{hostelId}/pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPending(int hostelId)
    {
        try
        {
            var tasks = await _cleaningService.GetPendingCleaningRecordsAsync(hostelId);
            return Ok(new { success = true, data = tasks });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending cleaning tasks for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving cleaning tasks" });
        }
    }

    /// <summary>
    /// Get cleaning history for a room
    /// </summary>
    [HttpGet("room/{roomId}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(int roomId, [FromQuery] int days = 30)
    {
        try
        {
            var history = await _cleaningService.GetCleaningHistoryAsync(roomId, days);
            return Ok(new { success = true, data = history });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cleaning history for room {RoomId}", roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving cleaning history" });
        }
    }

    /// <summary>
    /// Get cleaning record by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var record = await _cleaningService.GetCleaningRecordByIdAsync(id);
            if (record == null)
                return NotFound(new { success = false, message = "Cleaning record not found" });

            return Ok(new { success = true, data = record });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cleaning record {RecordId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving cleaning record" });
        }
    }

    /// <summary>
    /// Create a new cleaning record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateCleaningRecordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var record = await _cleaningService.CreateCleaningRecordAsync(request.RoomId, request.WorkerId);
            return CreatedAtAction(nameof(GetById), new { id = record.RecordId }, 
                new { success = true, data = record });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cleaning record");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error creating cleaning record" });
        }
    }

    /// <summary>
    /// Update cleaning record status
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCleaningStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var record = await _cleaningService.UpdateCleaningStatusAsync(
                id,
                request.Status,
                request.Remarks,
                request.WorkerId);

            if (record == null)
                return NotFound(new { success = false, message = "Cleaning record not found" });

            return Ok(new { success = true, data = record });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cleaning record {RecordId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error updating cleaning record" });
        }
    }

    /// <summary>
    /// Get daily cleaning report for a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}/report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetReport(int hostelId, [FromQuery] DateTime? date = null)
    {
        try
        {
            var report = await _cleaningService.GetCleaningReportAsync(hostelId, date ?? DateTime.UtcNow);
            return Ok(new { success = true, data = report });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cleaning report for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error generating cleaning report" });
        }
    }

    /// <summary>
    /// Get unclean rooms (not cleaned for X days)
    /// </summary>
    [HttpGet("hostel/{hostelId}/unclean")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUncleanRooms(int hostelId, [FromQuery] int thresholdDays = 3)
    {
        try
        {
            var rooms = await _cleaningService.GetUncleanRoomsAsync(hostelId, thresholdDays);
            return Ok(new { success = true, data = rooms });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unclean rooms for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving unclean rooms" });
        }
    }

    /// <summary>
    /// Delete a cleaning record
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _cleaningService.DeleteCleaningRecordAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Cleaning record not found" });

            return Ok(new { success = true, message = "Cleaning record deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cleaning record {RecordId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error deleting cleaning record" });
        }
    }
}
