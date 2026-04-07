using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHostelManagementSystem.Services.Interfaces;

namespace SmartHostelManagementSystem.Controllers.Api;

/// <summary>
/// API controller for dashboard analytics and metrics
/// </summary>
[ApiController]
[Route("api/hostels/{hostelId}/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard summary with overall metrics
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(int hostelId)
    {
        try
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync(hostelId);
            return Ok(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard summary for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving dashboard summary" });
        }
    }

    /// <summary>
    /// Get recent complaints
    /// </summary>
    [HttpGet("recent-complaints")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentComplaints(int hostelId, [FromQuery] int count = 10)
    {
        try
        {
            var complaints = await _dashboardService.GetRecentComplaintsAsync(hostelId, count);
            return Ok(new { success = true, data = complaints });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent complaints for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving recent complaints" });
        }
    }

    /// <summary>
    /// Get critical alerts
    /// </summary>
    [HttpGet("alerts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAlerts(int hostelId)
    {
        try
        {
            var alerts = await _dashboardService.GetCriticalAlertsAsync(hostelId);
            return Ok(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alerts for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving alerts" });
        }
    }

    /// <summary>
    /// Get room utilization metrics
    /// </summary>
    [HttpGet("metrics/utilization")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUtilizationMetrics(int hostelId)
    {
        try
        {
            var metrics = await _dashboardService.GetUtilizationMetricsAsync(hostelId);
            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving utilization metrics for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving utilization metrics" });
        }
    }

    /// <summary>
    /// Get complaint metrics
    /// </summary>
    [HttpGet("metrics/complaints")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetComplaintMetrics(int hostelId)
    {
        try
        {
            var metrics = await _dashboardService.GetComplaintMetricsAsync(hostelId);
            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complaint metrics for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving complaint metrics" });
        }
    }

    /// <summary>
    /// Get fee metrics
    /// </summary>
    [HttpGet("metrics/fees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeeMetrics(int hostelId)
    {
        try
        {
            var metrics = await _dashboardService.GetFeeMetricsAsync(hostelId);
            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fee metrics for hostel {HostelId}", hostelId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { success = false, message = "Error retrieving fee metrics" });
        }
    }
}
