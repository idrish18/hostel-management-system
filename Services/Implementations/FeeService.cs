using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IFeeService for fee and payment management
/// </summary>
public class FeeService : IFeeService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public FeeService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<FeeDto?> GetFeeByIdAsync(int feeId)
    {
        var fee = await _context.Fees
            .Include(f => f.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(f => f.FeeId == feeId && !f.IsDeleted);
        return fee == null ? null : MapToDto(fee);
    }

    public async Task<IEnumerable<FeeDto>> GetFeesByStudentAsync(int studentId)
    {
        var fees = await _context.Fees
            .Include(f => f.Student)
            .ThenInclude(s => s!.User)
            .Where(f => f.StudentId == studentId && !f.IsDeleted)
            .ToListAsync();
        return fees.Select(MapToDto);
    }

    public async Task<IEnumerable<FeeDto>> GetFeesByHostelAsync(int hostelId)
    {
        var fees = await _context.Fees
            .Include(f => f.Student)
            .ThenInclude(s => s!.User)
            .Where(f => f.HostelId == hostelId && !f.IsDeleted)
            .ToListAsync();
        return fees.Select(MapToDto);
    }

    public async Task<IEnumerable<FeeDto>> GetPendingFeesAsync(int hostelId)
    {
        var fees = await _context.Fees
            .Include(f => f.Student)
            .ThenInclude(s => s!.User)
            .Where(f => f.HostelId == hostelId && f.PaymentStatus != "Paid" && !f.IsDeleted)
            .ToListAsync();
        return fees.Select(MapToDto);
    }

    public async Task<IEnumerable<FeeDto>> GetOverdueFeesAsync(int hostelId)
    {
        var now = DateTime.UtcNow;
        var fees = await _context.Fees
            .Include(f => f.Student)
            .ThenInclude(s => s!.User)
            .Where(f => f.HostelId == hostelId && f.DueDate < now && f.PaymentStatus != "Paid" && !f.IsDeleted)
            .ToListAsync();
        return fees.Select(MapToDto);
    }

    public async Task<FeeDto> RecordFeeAsync(int studentId, decimal amount, string description, DateTime dueDate)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        var fee = new Fee
        {
            StudentId = studentId,
            HostelId = student?.HostelId ?? 0,
            Amount = amount,
            AmountPaid = 0,
            Description = description,
            PaymentStatus = "Pending",
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Fees.Add(fee);
        await _context.SaveChangesAsync();

        return new FeeDto
        {
            FeeId = fee.FeeId,
            StudentId = fee.StudentId,
            StudentName = student?.User?.FullName ?? "Unknown",
            Amount = fee.Amount,
            AmountPaid = fee.AmountPaid,
            Status = fee.PaymentStatus,
            DueDate = fee.DueDate,
            CreatedAt = fee.CreatedAt
        };
    }

    public async Task RecordPaymentAsync(int feeId, decimal paymentAmount)
    {
        var fee = await _context.Fees.FirstOrDefaultAsync(f => f.FeeId == feeId && !f.IsDeleted);
        if (fee == null) throw new InvalidOperationException("Fee not found");

        fee.AmountPaid += paymentAmount;
        if (fee.AmountPaid >= fee.Amount)
        {
            fee.PaymentStatus = "Paid";
            fee.AmountPaid = fee.Amount;
            fee.PaidDate = DateTime.UtcNow;
        }
        else if (fee.AmountPaid > 0)
        {
            fee.PaymentStatus = "Partial";
        }

        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalPendingFeesAsync(int hostelId)
    {
        return await _context.Fees
            .Where(f => f.HostelId == hostelId && f.PaymentStatus != "Paid" && !f.IsDeleted)
            .SumAsync(f => f.Amount - f.AmountPaid);
    }

    public async Task<decimal> GetTotalFeesCollectedAsync(int hostelId)
    {
        return await _context.Fees
            .Where(f => f.HostelId == hostelId && !f.IsDeleted)
            .SumAsync(f => f.AmountPaid);
    }

    public async Task<int> GetStudentsWithPendingFeesCountAsync(int hostelId)
    {
        return await _context.Fees
            .Where(f => f.HostelId == hostelId && f.PaymentStatus != "Paid" && !f.IsDeleted)
            .Select(f => f.StudentId)
            .Distinct()
            .CountAsync();
    }

    public async Task<string> GenerateReceiptAsync(int feeId)
    {
        var fee = await _context.Fees.FirstOrDefaultAsync(f => f.FeeId == feeId && !f.IsDeleted);
        if (fee == null) throw new InvalidOperationException("Fee not found");

        var transactionId = $"TXN-{feeId:D6}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        fee.TransactionId = transactionId;
        await _context.SaveChangesAsync();

        return transactionId;
    }

    public async Task<bool> DeleteFeeAsync(int feeId)
    {
        var fee = await _context.Fees.FirstOrDefaultAsync(f => f.FeeId == feeId && !f.IsDeleted);
        if (fee == null) return false;

        fee.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private static FeeDto MapToDto(Fee fee)
    {
        return new FeeDto
        {
            FeeId = fee.FeeId,
            StudentId = fee.StudentId,
            StudentName = fee.Student?.User?.FullName ?? "Unknown",
            Amount = fee.Amount,
            AmountPaid = fee.AmountPaid,
            Status = fee.PaymentStatus,
            DueDate = fee.DueDate,
            ReceiptNumber = fee.TransactionId,
            CreatedAt = fee.CreatedAt
        };
    }
}
