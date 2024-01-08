using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDB.Core.Abstraction;

namespace AIDB.Core.Models
{
    public class AiDbCommand : DbModel<string>
    {
        public string AiCommand { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}