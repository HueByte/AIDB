using AIDB.Core.Abstraction;

namespace AIDB.Core.Models;

public class Title : DbModel<int>
{
    public string? Name { get; set; }
}