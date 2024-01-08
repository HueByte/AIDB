using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories;

public class PersonRepository : BaseRepository<string, Person, AIDBMainContext>, IPersonRepository
{
    public PersonRepository(AIDBMainContext context) : base(context)
    {

    }
}