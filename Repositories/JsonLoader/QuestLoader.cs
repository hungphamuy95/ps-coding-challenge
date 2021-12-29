using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;

namespace Repositories.JsonLoader
{
    public class QuestLoader:IQuestLoader
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public QuestLoader(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IEnumerable<QuestModel> GetAllQuest()
        {
            return JsonConvert.DeserializeObject<IEnumerable<QuestModel>>(File.ReadAllText(_hostingEnvironment.ContentRootPath + @"\DataConfig.json"));
        }
    }
}
