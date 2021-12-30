﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IPlayerService _playerService;
        public ProgressController(IProgressService progressService, IPlayerService playerService)
        {
            _progressService = progressService;
            _playerService = playerService;
        }
        [HttpPost]
        public async Task<IActionResult> GetProgress([FromBody]ProgressRequestModel request)
        {
            if (ModelState.IsValid)
            {
                var isExistedPlayer = await _playerService.ValidatePlayerExisted(request.PlayerId);
                if (isExistedPlayer)
                {
                    var res = await _progressService.GetProcess(request);
                    return (res == null) ? StatusCode(500, "Internal Server Error") : Ok(res);
                }
                return BadRequest("Player could not be found");
            }
            return BadRequest(ModelState);
        }
    }
}
