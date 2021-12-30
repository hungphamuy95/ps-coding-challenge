using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
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
        private readonly IBaseRepository<PlayerQuest> _playerQuestRepository;
        private readonly IBaseRepository<PlayerMilestone> _playerMilestoneRepository;
        private readonly IQuestLoader _questLoader;
        private readonly ICommonMethod _commonMethod;
        private readonly BetValueConfig _setting;
        readonly ILogger<ProgressService> _logger;

        public ProgressService(IBaseRepository<Player> playerRepository,
            IBaseRepository<PlayerQuest> playerQuestRepository,
            IBaseRepository<PlayerMilestone> playerMilestoneRepository,
            IQuestLoader questLoader,
            ICommonMethod utilities,
            IOptions<BetValueConfig> setting, ILogger<ProgressService> logger)
        {
            _playerRepository = playerRepository;
            _playerQuestRepository = playerQuestRepository;
            _playerMilestoneRepository = playerMilestoneRepository;
            _questLoader = questLoader;
            _commonMethod = utilities;
            _setting = setting.Value;
            _logger = logger;
        }

        public async Task<ProgressResponseModel> Process(ProgressRequestModel req)
        {
            try
            {
                _logger.LogInformation(JsonSerializer.Serialize(req));
                // Calculate Questpoint based on config value and parameters
                var questPoint = (req.ChipAmountBet * _setting.RateFromBet) + (req.PlayerLevel * _setting.LevelBonusRate);
                var questPointEarn = questPoint;

                var allQuests = _questLoader.GetAllQuest().OrderBy(x => x.PassingPoint);
                
                foreach (var quest in allQuests)
                {
                    if (await _playerQuestRepository.Exist(x=>x.PlayerId == req.PlayerId && x.QuestId == quest.QuestID))
                    {
                        continue;
                    }
                    foreach (var milestone in quest.Milestones)
                    {
                        if(await _playerMilestoneRepository.Exist(x => x.PlayerId == req.PlayerId && x.MilestoneIndex == milestone.MilestoneIndex))
                            continue;
                        if (questPoint >= milestone.GoalPoint)
                        {
                            questPoint += milestone.AwardChip;
                            questPointEarn += milestone.AwardChip;
                            _playerMilestoneRepository.Add(new PlayerMilestone
                            {
                                PlayerId = req.PlayerId,
                                CreateDate = DateTime.Now,
                                Id = Guid.NewGuid(),
                                MilestoneIndex = milestone.MilestoneIndex,
                                ChipsAwarded = milestone.AwardChip
                            });
                        }
                    }

                    if (questPoint >= quest.PassingPoint)
                    {
                        _playerQuestRepository.Add(new PlayerQuest
                        {
                            PlayerId = req.PlayerId,
                            QuestId = quest.QuestID,
                            CreatedDate = DateTime.Now,
                            Id = Guid.NewGuid()
                        });
                        questPoint -= quest.PassingPoint;
                    }else
                        break;
                }

                await _playerMilestoneRepository.SaveChangesAsync();
                await _playerQuestRepository.SaveChangesAsync();

                var completedQuest =
                    await _playerQuestRepository.GetAsQueryable().CountAsync(x => x.PlayerId == req.PlayerId);

                int percentComplete = (int)Math.Round((double)(100 * completedQuest) / _questLoader.GetAllQuest().Count());

                var completedMilestones = await (from a in _playerMilestoneRepository.GetAsQueryable()
                        where a.PlayerId == req.PlayerId
                        select new MileStoneCompletedModel
                        {
                            ChipsAwarded = a.ChipsAwarded,
                            MilestoneIndex = a.MilestoneIndex
                        }
                    ).ToArrayAsync();

                return new ProgressResponseModel
                {
                    QuestPointsEarned = questPointEarn,
                    TotalQuestPercentCompleted = percentComplete,
                    MilestonesCompleted = completedMilestones
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
