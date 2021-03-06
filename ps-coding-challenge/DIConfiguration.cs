using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Repositories;
using Repositories.Entities;
using Repositories.JsonLoader;
using Services.Interfaces;
using Services.Implements;
using Utilities.Interfaces;

namespace ps_coding_challenge
{
    public class DiConfiguration
    {
        public static void ConfigServices(IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IBaseRepository<Player>, BaseRepository<Player>>();
            services.AddScoped<IBaseRepository<PlayerQuest>, BaseRepository<PlayerQuest>>();
            services.AddScoped<IBaseRepository<PlayerMilestone>, BaseRepository<PlayerMilestone>>();
            // Register Json loader
            services.AddScoped<IQuestLoader,QuestLoader>();
            // Register Services
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<IStateService, StateService>();
            // Register Utilities
            services.AddScoped<ICommonMethod, Utilities.Implements.CommonMethod>();
        }
    }
}
