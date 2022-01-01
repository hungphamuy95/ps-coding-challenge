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
using Repositories.JsonLoader;
using Services.Interfaces;
using Utilities.Interfaces;

namespace Services.Implements
{
    public class StateService : IStateService
    {
        private readonly IBaseRepository<PlayerQuest> _playerQuestRepository;
        private readonly IBaseRepository<PlayerMilestone> _playerMilestoneRepository;
        private readonly IQuestLoader _questLoader;
        private readonly ILogger<StateService> _logger;
        public StateService(IBaseRepository<PlayerQuest> playerQuestRepository, IBaseRepository<PlayerMilestone> playerMilestoneRepository, ILogger<StateService> logger, IQuestLoader questLoader)
        {
            _playerQuestRepository = playerQuestRepository;
            _logger = logger;
            _questLoader = questLoader;
            _playerMilestoneRepository = playerMilestoneRepository;
        }
        public async Task<StateResponseModel> GetState(string playerId)
        {
            try
            {
                // Get all completed quests by player id
                var completedQuest =
                    await _playerQuestRepository.Count(x => x.PlayerId == playerId);

                // Calculate the percentage of completed quests and get the all completed milestones
                var percentComplete = (int)Math.Round((double)(100 * completedQuest) / _questLoader.GetAllQuest().Count());

                // Get last completed milestone by created datetime
                var lastMilestoneIndexCompleted =  _playerMilestoneRepository.GetAsQueryable()
                    .Where(x => x.PlayerId == playerId)
                    .OrderByDescending(x => x.CreateDate).FirstOrDefault();
                return new StateResponseModel
                {
                    TotalQuestPercentCompleted = percentComplete,
                    LastMilestoneIndexCompleted = lastMilestoneIndexCompleted?.MilestoneIndex ?? 0
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
