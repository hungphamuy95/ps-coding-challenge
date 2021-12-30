using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Repositories;
using Repositories.Entities;
using Services.Interfaces;
using Utilities.Interfaces;

namespace Services.Implements
{
    public class StateService : IStateService
    {
        private readonly ICommonMethod _utilities;
        private readonly IBaseRepository<PlayerQuestState> _playerQuestStateRepository;
        private readonly ILogger<StateService> _logger;
        public StateService(ICommonMethod utilities, IBaseRepository<PlayerQuestState> playerQuestStateRepository, ILogger<StateService> logger)
        {
            _utilities = utilities;
            _playerQuestStateRepository = playerQuestStateRepository;
            _logger = logger;
        }
        public async Task<StateResponseModel> GetState(string playerId)
        {
            try
            {
                var totalPercentQuestDone = await _utilities.CalculatePercentQuestByPlayer(playerId);
                var lastMilestoneIndexCompleted = await _playerQuestStateRepository.GetAsQueryable()
                    .OrderByDescending(x => x.CreatedDate)
                    .Where(y => y.PlayerId == playerId && y.IsCompletedMilestone).Select(z => z.MileStoneIndex).FirstOrDefaultAsync();
                return new StateResponseModel
                {
                    TotalQuestPercentCompleted = totalPercentQuestDone,
                    LastMilestoneIndexCompleted = lastMilestoneIndexCompleted
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException != null
                    ? $"An error has occurred at StateService.GetState : {ex.InnerException.Message}"
                    : $"An error has occurred at StateService.GetState: {ex.Message}");
                return null;
            }
        }
    }
}
