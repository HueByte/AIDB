using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories;

public class TeacherRepository : BaseRepository<string, Teacher, AIDBMainContext>, ITeacherRepository
{
    public TeacherRepository(AIDBMainContext context) : base(context)
    {

    }
}