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


namespace ps_coding_challenge
{
    public class DiConfiguration
    {
        public static void ConfigServices(IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<Player>, BaseRepository<Player>>();
            services.AddScoped<IBaseRepository<PlayerQuestState>, BaseRepository<PlayerQuestState>>();
            services.AddScoped<IQuestLoader,QuestLoader>();
            services.AddScoped<IProgressService, ProgressService>();
        }
    }
}
