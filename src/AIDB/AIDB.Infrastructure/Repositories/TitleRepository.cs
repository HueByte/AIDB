using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories;

public class TitleRepository : BaseRepository<int, Title, AIDBMainContext>, ITitleRepository
{
    public TitleRepository(AIDBMainContext context) : base(context)
    {

    }
}