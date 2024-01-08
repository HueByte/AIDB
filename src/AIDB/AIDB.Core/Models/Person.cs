using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Person : DbModel<string>
{
    public string? Name { get; set; }
    public string? MiddleName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public virtual IList<Subject> Subjects { get; set; } = new List<Subject>();
}