using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDB.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIDB.App.Controllers
{
    public class RawSqlController : BaseController
    {
        private readonly IRawSqlExecutor _rawSqlExecutor;

        public RawSqlController(IRawSqlExecutor rawSqlExecutor)
        {
            _rawSqlExecutor = rawSqlExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteRawSql([FromBody] string sql)
        {
            var result = await _rawSqlExecutor.ExecuteRawSqlAsync(sql);

            return Ok(result);
        }
    }
}