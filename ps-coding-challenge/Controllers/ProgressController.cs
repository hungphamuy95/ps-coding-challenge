using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Services.Interfaces;

namespace ps_coding_challenge.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ProgressController : Controller
    {
        private readonly IProgressService _progressService;
        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }
        [HttpPost]
        public async Task<ProgressResponseModel> GetProgress([FromBody]ProgressRequestModel req)
        {
            return await _progressService.GetProcess(req);
        }
    }
}
