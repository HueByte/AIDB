using System.ComponentModel.DataAnnotations.Schema;
using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Teacher : DbModel<string>
{
    public virtual Title? Title { get; set; }
    public virtual Person Person { get; set; } = default!;
}