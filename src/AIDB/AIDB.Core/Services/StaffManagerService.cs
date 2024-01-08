using AIDB.Core.IRepositories;
using AIDB.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AIDB.Core.Services;

public class StaffManagerService : IStaffManagerService
{
    private readonly ITeacherRepository _teacherRepository;
    public StaffManagerService(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<List<Teacher>> GetTeachersAsync()
    {
        return await _teacherRepository.AsQueryable()
            .Include(t => t.Person)
            .ThenInclude(ps => ps.Subjects)
            .Include(t => t.Title)
            .ToListAsync();
    }

    public async Task AddStaff()
    { }
}

public interface IStaffManagerService
{
    Task<List<Teacher>> GetTeachersAsync();
}