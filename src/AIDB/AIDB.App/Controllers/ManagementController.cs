using AIDB.App.ViewModels;
using AIDB.Core.Models;
using AIDB.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIDB.App.Controllers;

public class ManagementController : BaseController
{
    private readonly IStaffManagerService _staffManagerService;
    private readonly IPersonManagerService _personManagerService;
    public ManagementController(IStaffManagerService staffManagerService, IPersonManagerService personManagerService)
    {
        _staffManagerService = staffManagerService;
        _personManagerService = personManagerService;
    }

    [HttpGet("Teachers")]
    public async Task<IActionResult> GetTeachers()
    {
        var staff = await _staffManagerService.GetTeachersAsync();
        var staffView = staff.Select(s => new TeacherViewModel
        {
            Address = s.Person.Address,
            Email = s.Person.Email,
            MiddleName = s.Person.MiddleName,
            Name = s.Person.Name,
            Surname = s.Person.Surname,
            Title = s.Title?.Name,
            Subjects = s.Person.Subjects.Select(s => new SubjectViewModel
            {
                Name = s.Name,
            }).ToList()
        });

        return Ok(staffView);
    }
}