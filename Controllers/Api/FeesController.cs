using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Models.DTOs.Requests;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for managing fees and payments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeesController : ControllerBase
{
    private readonly IFeeService _feeService;
    private readonly ILogger<FeesController> _logger;

    public FeesController(IFeeService feeService, ILogger<FeesController> logger)
    {
        _feeService = feeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all fees for a student
    /// </summary>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        try
        {
            var fees = await _feeService.GetFeesByStudentAsync(studentId);
            return Ok(new { success = true, data = fees });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fees for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving fees" });
        }
    }

    /// <summary>
    /// Get all fees for a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByHostel(int hostelId)
    {
        try
        {
            var fees = await _feeService.GetFeesByHostelAsync(hostelId);
            return Ok(new { success = true, data = fees });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fees for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving fees" });
        }
    }

    /// <summary>
    /// Get pending fees for a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}/pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPending(int hostelId)
    {
        try
        {
            var fees = await _feeService.GetPendingFeesAsync(hostelId);
            return Ok(new { success = true, data = fees });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending fees for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving pending fees" });
        }
    }

    /// <summary>
    /// Get overdue fees for a hostel
    /// </summary>
    [HttpGet("hostel/{hostelId}/overdue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOverdue(int hostelId)
    {
        try
        {
            var fees = await _feeService.GetOverdueFeesAsync(hostelId);
            return Ok(new { success = true, data = fees });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue fees for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving overdue fees" });
        }
    }

    /// <summary>
    /// Get fee by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var fee = await _feeService.GetFeeByIdAsync(id);
            if (fee == null)
                return NotFound(new { success = false, message = "Fee not found" });

            return Ok(new { success = true, data = fee });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fee {FeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving fee" });
        }
    }

    /// <summary>
    /// Record a new fee
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RecordFee([FromBody] RecordFeeRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var fee = await _feeService.RecordFeeAsync(
                request.StudentId,
                request.Amount,
                request.Description,
                request.DueDate);

            return CreatedAtAction(nameof(GetById), new { id = fee.FeeId }, 
                new { success = true, data = fee });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording fee");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error recording fee" });
        }
    }

    /// <summary>
    /// Record a payment for a fee
    /// </summary>
    [HttpPost("{feeId}/payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RecordPayment(int feeId, [FromBody] RecordPaymentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            await _feeService.RecordPaymentAsync(feeId, request.PaymentAmount);
            return Ok(new { success = true, message = "Payment recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment for fee {FeeId}", feeId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error recording payment" });
        }
    }

    /// <summary>
    /// Generate receipt for a fee
    /// </summary>
    [HttpGet("{id}/receipt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateReceipt(int id)
    {
        try
        {
            var receipt = await _feeService.GenerateReceiptAsync(id);
            return Ok(new { success = true, data = new { receipt } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating receipt for fee {FeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error generating receipt" });
        }
    }

    /// <summary>
    /// Delete a fee
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _feeService.DeleteFeeAsync(id);
            if (!result)
                return NotFound(new { success = false, message = "Fee not found" });

            return Ok(new { success = true, message = "Fee deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fee {FeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error deleting fee" });
        }
    }
}
