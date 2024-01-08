using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories
{
    public class AiDbCommandsRepository : BaseRepository<string, AiDbCommand, AIDBMainContext>, IAiDbCommandRepository
    {
        public AiDbCommandsRepository(AIDBMainContext context) : base(context)
        {

        }
    }
}