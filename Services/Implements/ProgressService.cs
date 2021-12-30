using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Repositories;
using Repositories.Entities;
using Repositories.JsonLoader;
using Services.Interfaces;
using Utilities.Interfaces;

namespace Services.Implements
{
    public class ProgressService : IProgressService
    {
        private readonly IBaseRepository<Player> _playerRepository;
        private readonly IBaseRepository<PlayerQuestState> _playerQuestStateRepository;
        private readonly IQuestLoader _questLoader;
        private readonly ICommonMethod _commonMethod;
        private readonly BetValueConfig _setting;
        readonly ILogger<ProgressService> _logger;

        public ProgressService(IBaseRepository<Player> playerRepository,
            IBaseRepository<PlayerQuestState> playerQuestStateRepository, IQuestLoader questLoader,
            ICommonMethod utilities,
            IOptions<BetValueConfig> setting, ILogger<ProgressService> logger)
        {
            _playerRepository = playerRepository;
            _playerQuestStateRepository = playerQuestStateRepository;
            _questLoader = questLoader;
            _commonMethod = utilities;
            _setting = setting.Value;
            _logger = logger;
        }

        public async Task<ProgressResponseModel> GetProcess(ProgressRequestModel req)
        {
            try
            {
                // Calculate Questpoint based on config value and parameters
                var questPoint = (req.ChipAmountBet * _setting.RateFromBet) + (req.PlayerLevel * _setting.LevelBonusRate);

                // Due to the milestone index could be duplicated over the quests so need to join the id of quest and index of milestone
                var allMileStone = (from q in _questLoader.GetAllQuest()
                        from m in q.Milestones
                        select new
                        {
                            index = $"{q.QuestID},{m.MilestoneIndex}",
                            award = m.AwardChip
                        }
                    ).ToDictionary(e => e.index, e => e.award);

                // Due to the milestone index could be duplicated over the quests so need to join the id of quest and index of milestone
                var doneMilestonesIndex = await (from a in _playerQuestStateRepository.GetAsQueryable()
                        where
                            a.PlayerId == req.PlayerId && a.IsCompletedMilestone
                        select $"{a.QuestId},{a.MileStoneIndex}"
                    ).ToListAsync();

                var mileStoneCompleted = (from b in allMileStone
                        where doneMilestonesIndex.Contains(b.Key)
                        select new MileStoneCompletedModel
                        {
                            MilestoneIndex = int.Parse(b.Key.ToString().Split(',')[1]),
                            ChipsAwarded = b.Value
                        }
                    ).ToArray();

                var percentQuestCompleted = await _commonMethod.CalculatePercentQuestByPlayer(req.PlayerId);

                return new ProgressResponseModel
                {
                    QuestPointsEarned = questPoint,
                    TotalQuestPercentCompleted = percentQuestCompleted,
                    MilestonesCompleted = mileStoneCompleted
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException != null
                    ? $"An error has occurred at ProgressService.GetProcess : {ex.InnerException.Message}"
                    : $"An error has occurred at ProgressService.GetProcess: {ex.Message}");
                return null;
            }
        }
    }
}
