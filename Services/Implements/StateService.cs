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
        private readonly ICommonMethod _utilities;
        private readonly IBaseRepository<PlayerQuest> _playerQuestRepository;
        private readonly IBaseRepository<PlayerMilestone> _playerMilestoneRepository;
        private readonly IQuestLoader _questLoader;
        private readonly ILogger<StateService> _logger;
        public StateService(ICommonMethod utilities, IBaseRepository<PlayerQuest> playerQuestRepository, IBaseRepository<PlayerMilestone> playerMilestoneRepository, ILogger<StateService> logger, IQuestLoader questLoader)
        {
            _utilities = utilities;
            _playerQuestRepository = playerQuestRepository;
            _logger = logger;
            _questLoader = questLoader;
            _playerMilestoneRepository = playerMilestoneRepository;
        }
        public async Task<StateResponseModel> GetState(string playerId)
        {
            try
            {
                var completedQuest =
                    await _playerQuestRepository.GetAsQueryable().CountAsync(x => x.PlayerId == playerId);

                var percentComplete = (int)Math.Round((double)(100 * completedQuest) / _questLoader.GetAllQuest().Count());
                var lastMilestoneIndexCompleted = await _playerMilestoneRepository.GetAsQueryable()
                    .OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync();
                return new StateResponseModel
                {
                    TotalQuestPercentCompleted = percentComplete,
                    LastMilestoneIndexCompleted = lastMilestoneIndexCompleted.MilestoneIndex
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
