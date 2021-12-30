using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.JsonLoader;
using Utilities.Interfaces;

namespace Utilities.Implements
{
    public class CommonMethod : ICommonMethod
    {
        private readonly IBaseRepository<Player> _playerRepository;
        private readonly IBaseRepository<PlayerQuestState> _playerQuestStateRepository;
        private readonly IQuestLoader _questLoader;

        public CommonMethod(IBaseRepository<Player> playerRepository,
            IBaseRepository<PlayerQuestState> playerQuestStateRepository, IQuestLoader questLoader)
        {
            _playerRepository = playerRepository;
            _playerQuestStateRepository = playerQuestStateRepository;
            _questLoader = questLoader;
        }


        public async Task<int> CalculatePercentQuestByPlayer(string playerId)
        {
            var allQuests = (from q in _questLoader.GetAllQuest()
                select new { questId = q.QuestID, goal = q.Milestones.Count }).ToDictionary(e => e.questId, e => e.goal);

            var questsByPlayer = await(from m in _playerQuestStateRepository.GetAsQueryable()
                where m.PlayerId == playerId
                                       group m by m.QuestId
                into g
                select new
                {
                    questStateId = g.Key,
                    completedMilestones = g.Count(x => x.IsCompletedMilestone)
                }).ToArrayAsync();

            var totalQuest = _questLoader.GetAllQuest().Count();
            var totalQuestCompleted = questsByPlayer.Count(item => allQuests.ContainsKey(item.questStateId) && allQuests[item.questStateId] == item.completedMilestones);
            return (int) Math.Round((double) (100 * totalQuestCompleted) / totalQuest);
        }
    }
}
