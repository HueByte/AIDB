using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories;

public class GradeRepository : BaseRepository<string, Grade, AIDBMainContext>, IGradeRepository
{
    public GradeRepository(AIDBMainContext context) : base(context)
    {

    }
}