using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Models;

namespace Repositories.JsonLoader
{
    public interface IQuestLoader
    {
        IEnumerable<QuestModel> GetAllQuest();
    }
}
