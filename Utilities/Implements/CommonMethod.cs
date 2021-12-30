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
        private readonly IBaseRepository<PlayerQuest> _playerQuestStateRepository;
        private readonly IQuestLoader _questLoader;

        public CommonMethod(IBaseRepository<Player> playerRepository,
            IBaseRepository<PlayerQuest> playerQuestStateRepository, IQuestLoader questLoader)
        {
            _playerRepository = playerRepository;
            _playerQuestStateRepository = playerQuestStateRepository;
            _questLoader = questLoader;
        }


        public async Task<int> CalculatePercentQuestByPlayer(string playerId)
        {
            return 1;
        }
    }
}
