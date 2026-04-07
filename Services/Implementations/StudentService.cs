using SmartHostelManagementSystem.Data;
using SmartHostelManagementSystem.Models.DTOs.Responses;
using SmartHostelManagementSystem.Models.Entities;
using SmartHostelManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartHostelManagementSystem.Services.Implementations;

/// <summary>
/// Implementation of IStudentService for student management
/// </summary>
public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly IRoomService _roomService;

    public StudentService(ApplicationDbContext context, ICacheService cacheService, IRoomService roomService)
    {
        _context = context;
        _cacheService = cacheService;
        _roomService = roomService;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
        if (student == null) return null;

        return MapToDto(student);
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsByHostelAsync(int hostelId)
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.HostelId == hostelId && !s.IsDeleted)
            .ToListAsync();
        return students.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsByRoomAsync(int roomId)
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.RoomId == roomId && !s.IsDeleted)
            .ToListAsync();
        return students.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentDto>> GetUnassignedStudentsAsync(int hostelId)
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.HostelId == hostelId && s.RoomId == null && !s.IsDeleted)
            .ToListAsync();
        return students.Select(MapToDto);
    }

    public async Task AssignStudentToRoomAsync(int studentId, int roomId)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
        if (student == null) throw new InvalidOperationException("Student not found");

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && !r.IsDeleted);
        if (room == null) throw new InvalidOperationException("Room not found");

        if (room.CurrentOccupancy >= room.Capacity)
            throw new InvalidOperationException("Room is at full capacity");

        student.RoomId = roomId;
        room.CurrentOccupancy++;
        await _context.SaveChangesAsync();
    }

    public async Task UnassignStudentAsync(int studentId)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
        if (student == null) return;

        if (student.RoomId.HasValue)
        {
            await _roomService.DecrementOccupancyAsync(student.RoomId.Value);
        }

        student.RoomId = null;
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetStudentCountAsync(int hostelId)
    {
        return await _context.Students.CountAsync(s => s.HostelId == hostelId && !s.IsDeleted);
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            StudentId = student.StudentId,
            RollNumber = student.RollNumber,
            FullName = student.User?.FullName ?? "Unknown",
            Email = student.User?.Email ?? "N/A",
            IsAssignedToRoom = student.RoomId.HasValue,
            RoomId = student.RoomId
        };
    }
}
