using System.ComponentModel.DataAnnotations.Schema;
using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Grade : DbModel<string>
{
    public int Score { get; set; }
    public DateTime Date { get; set; }
    public virtual Student Student { get; set; } = default!;
    public virtual Subject Subject { get; set; } = default!;
    public virtual Teacher Teacher { get; set; } = default!;
}