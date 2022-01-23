using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Folders;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FolderController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] Create.Command query)
        {
            return HandleResult(await Mediator.Send(query));
        }
    }
}