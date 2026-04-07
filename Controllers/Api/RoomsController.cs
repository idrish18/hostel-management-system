using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing rooms
/// </summary>
[ApiController]
[Route("api/hostels/{hostelId}/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    /// <summary>
    /// Get all rooms for a hostel
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllByHostel(int hostelId)
    {
        try
        {
            var rooms = await _roomService.GetRoomsByHostelAsync(hostelId);
            return Ok(new { success = true, data = rooms });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rooms for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving rooms" });
        }
    }

    /// <summary>
    /// Get available rooms for a hostel
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvailable(int hostelId)
    {
        try
        {
            var rooms = await _roomService.GetAvailableRoomsAsync(hostelId);
            return Ok(new { success = true, data = rooms });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available rooms for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving available rooms" });
        }
    }

    /// <summary>
    /// Get room by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int hostelId, int id)
    {
        try
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
                return NotFound(new { success = false, message = "Room not found" });

            return Ok(new { success = true, data = room });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving room {RoomId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving room" });
        }
    }

    /// <summary>
    /// Create a new room
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(int hostelId, [FromBody] CreateUpdateRoomRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var room = await _roomService.CreateRoomAsync(hostelId, request.RoomNumber, request.Capacity);
            return CreatedAtAction(nameof(GetById), new { hostelId, id = room.RoomId }, 
                new { success = true, data = room });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error creating room" });
        }
    }

    /// <summary>
    /// Update a room
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int hostelId, int id, [FromBody] CreateUpdateRoomRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var room = await _roomService.UpdateRoomAsync(id, request.RoomNumber, request.Capacity);
            if (room == null)
                return NotFound(new { success = false, message = "Room not found" });

            return Ok(new { success = true, data = room });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room {RoomId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error updating room" });
        }
    }

    /// <summary>
    /// Delete a room
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int hostelId, int id)
    {
        try
        {
            var result = await _roomService.DeleteRoomAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Room not found" });

            return Ok(new { success = true, message = "Room deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting room {RoomId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error deleting room" });
        }
    }

    /// <summary>
    /// Check if room is full
    /// </summary>
    [HttpGet("{id}/occupancy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckOccupancy(int hostelId, int id)
    {
        try
        {
            var isFull = await _roomService.IsRoomFullAsync(id);
            return Ok(new { success = true, isFull });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking room occupancy {RoomId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error checking room occupancy" });
        }
    }
}
