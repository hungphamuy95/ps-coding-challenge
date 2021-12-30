using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Repositories;
using Repositories.Entities;
using Services.Interfaces;

namespace Services.Implements
{
    public class PlayerService : IPlayerService
    {
        private readonly IBaseRepository<Player> _playerRepository;
        private readonly ILogger<PlayerService> _logger;
        public PlayerService(IBaseRepository<Player> playerRepository, ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<bool> ValidatePlayerExisted(string playerId)
        {
            try
            {
                return await _playerRepository.Exist(x => x.PlayerId == playerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException != null
                    ? $"An error has occurred at PlayerService.ValidatePlayerExisted : {ex.InnerException.Message}"
                    : $"An error has occurred at PlayerService.ValidatePlayerExisted: {ex.Message}");
                throw;
            }
        }
    }
}
