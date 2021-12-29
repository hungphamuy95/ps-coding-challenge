using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Repositories;
using Repositories.Entities;
using Repositories.JsonLoader;
using Services.Interfaces;

namespace Services.Implements
{
    public class ProgressService : IProgressService
    {
        private readonly IBaseRepository<Player> _playerRepository;
        private readonly IBaseRepository<PlayerQuestState> _playerQuestStateRepository;
        private readonly IQuestLoader _questLoader;
        private readonly BetValueConfig _setting;

        public ProgressService(IBaseRepository<Player> playerRepository,
            IBaseRepository<PlayerQuestState> playerQuestStateRepository, IQuestLoader questLoader,
            IOptions<BetValueConfig> setting)
        {
            _playerRepository = playerRepository;
            _playerQuestStateRepository = playerQuestStateRepository;
            _questLoader = questLoader;
            _setting = setting.Value;
        }

        public async Task<ProgressResponseModel> GetProcess(ProgressRequestModel req)
        {
            var questPoint = (req.ChipAmountBet * _setting.RateFromBet) + (req.PlayerLevel * _setting.LevelBonusRate);
            var allQuests = (from q in _questLoader.GetAllQuest()
                select new {questId = q.QuestID, goal = q.Milestones.Count}).ToDictionary(e=>e.questId, e=>e.goal);

            var allMileStone = (from q in _questLoader.GetAllQuest()
                    from m in q.Milestones
                    select new
                    {
                        index = $"{q.QuestID},{m.MilestoneIndex}",
                        award = m.AwardChip
                    }
                ).ToDictionary(e => e.index, e => e.award);
            

            var questsByPlayer = await (from m in _playerQuestStateRepository.GetAsQueryable()
                where m.PlayerId == req.PlayerId
                group m by m.QuestId
                into g
                select new
                {
                    questStateId = g.Key,
                    completedMilestones = g.Count(x => x.IsCompletedMilestone)
                }).ToArrayAsync();

            var totalQuest = _questLoader.GetAllQuest().Count();
            var totalQuestCompleted = questsByPlayer.Count(item => allQuests.ContainsKey(item.questStateId) && allQuests[item.questStateId] == item.completedMilestones);

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

            return new ProgressResponseModel
            {
                QuestPointsEarned = questPoint,
                TotalQuestPercentCompleted = (int)Math.Round((double)(100 * totalQuestCompleted) / totalQuest),
                MilestonesCompleted = mileStoneCompleted
            };

        }
    }
}
