using System.ComponentModel.DataAnnotations.Schema;
using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Student : DbModel<string>
{
    public DateTime StartYear { get; set; }
    public virtual Person Person { get; set; } = default!;
    public virtual IList<Grade>? Grades { get; set; }
}