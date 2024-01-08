using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories;

public class SubjectRepository : BaseRepository<int, Subject, AIDBMainContext>, ISubjectRepository
{
    public SubjectRepository(AIDBMainContext context) : base(context)
    {

    }
}