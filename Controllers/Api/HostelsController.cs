using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing hostels
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HostelsController : ControllerBase
{
    private readonly IHostelService _hostelService;
    private readonly ILogger<HostelsController> _logger;

    public HostelsController(IHostelService hostelService, ILogger<HostelsController> logger)
    {
        _hostelService = hostelService;
        _logger = logger;
    }

    /// <summary>
    /// Get all hostels
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var hostels = await _hostelService.GetAllHostelsAsync();
            return Ok(new { success = true, data = hostels });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hostels");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving hostels" });
        }
    }

    /// <summary>
    /// Get hostel by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hostel = await _hostelService.GetHostelByIdAsync(id);
            if (hostel == null)
                return NotFound(new { success = false, message = "Hostel not found" });

            return Ok(new { success = true, data = hostel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hostel {HostelId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving hostel" });
        }
    }

    /// <summary>
    /// Create a new hostel
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateUpdateHostelRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var hostel = await _hostelService.CreateHostelAsync(
                request.Name, 
                request.Location, 
                request.Description, 
                request.PhoneNumber,
                request.TotalRooms);

            return CreatedAtAction(nameof(GetById), new { id = hostel.HostelId }, 
                new { success = true, data = hostel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hostel");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error creating hostel" });
        }
    }

    /// <summary>
    /// Update a hostel
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateHostelRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var hostel = await _hostelService.UpdateHostelAsync(
                id,
                request.Name,
                request.Location,
                request.Description,
                request.PhoneNumber);

            if (hostel == null)
                return NotFound(new { success = false, message = "Hostel not found" });

            return Ok(new { success = true, data = hostel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hostel {HostelId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error updating hostel" });
        }
    }

    /// <summary>
    /// Delete a hostel
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _hostelService.DeleteHostelAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Hostel not found" });

            return Ok(new { success = true, message = "Hostel deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hostel {HostelId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error deleting hostel" });
        }
    }
}
