using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Subject : DbModel<int>
{
    public string? Name { get; set; }
    public virtual IList<Person> Persons { get; set; } = new List<Person>();
}