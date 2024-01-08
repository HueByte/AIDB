using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDB.Core.Abstraction;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;

namespace AIDB.Infrastructure.Repositories
{
    public class StudentRepository : BaseRepository<string, Student, AIDBMainContext>, IStudentRepository
    {
        public StudentRepository(AIDBMainContext context) : base(context)
        {

        }
    }
}