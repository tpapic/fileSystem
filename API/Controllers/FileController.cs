using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Files;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FileController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetFiles([FromQuery] Search.Query query)
        {
            return HandleResult(await Mediator.Send(query));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] Create.Command query)
        {
            return HandleResult(await Mediator.Send(query));
        }
    }
}