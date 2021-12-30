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
    public class StateController : Controller
    {
        private readonly IStateService _stateService;
        private readonly IPlayerService _playerService;
        public StateController(IStateService stateService, IPlayerService playerService)
        {
            _stateService = stateService;
            _playerService = playerService;
        }

        public async Task<IActionResult> GetState(string playerId)
        {
            if (!string.IsNullOrEmpty(playerId))
            {
                var checkExistedPlayer = await _playerService.ValidatePlayerExisted(playerId);
                if (checkExistedPlayer)
                {
                    var res = await _stateService.GetState(playerId);
                    return (res == null) ? StatusCode(500, "Internal Server Error") : Ok(res);
                }
                return BadRequest("Player could not be found");
            }
            return BadRequest("Player ID could not be null or empty");
        }
    }
}
