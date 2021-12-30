using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;
using Repositories.Entities;
using Models;
using Serilog;

namespace ps_coding_challenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Use an in-memory database for quick dev and testing
            // TODO: Swap out with a real database in production
            services.AddDbContext<PlayStudioContext>(opt => opt.UseInMemoryDatabase("PlayStudio"));
            //services.AddDbContext<PlayStudioContext>(opt => opt.UseSqlServer(""));

            services.AddControllers();
            
          
            // api document
            services.AddSwaggerDocument();
            // api version
            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new MediaTypeApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            });

            // auto mapper

            // DI Register
            DiConfiguration.ConfigServices(services);
            
            // Get Config
            services.Configure<BetValueConfig>(options =>
            {
                options.LevelBonusRate= int.Parse(Configuration.GetSection("CommonConfig:LevelBonusRate").Value);
                options.RateFromBet = int.Parse(Configuration.GetSection("CommonConfig:RateFromBet").Value);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<PlayStudioContext>();
                    SeedData(context);
                }
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            //app.UseCors("CorsPolicy");
            app.UseApiVersioning();
            app.UseOpenApi();
            app.UseSwaggerUi3();

        }

        private static void SeedData(PlayStudioContext context)
        {
            context.Players.Add(new Player
            {
                PlayerId = "Shround",
                PlayerLevel = 1,
                PlayerQuestStates = new List<PlayerQuestState>
                {
                    new PlayerQuestState()
                    {
                        PlayerId = "Shround",
                        Id = 1,
                        CreatedDate = DateTime.Now.AddHours(-5),
                        IsCompletedMilestone = false,
                        QuestId = 1,
                        MileStoneIndex = 1
                    },
                    new PlayerQuestState()
                    {
                        PlayerId = "Shround",
                        Id = 2,
                        CreatedDate = DateTime.Now.AddHours(-3),
                        IsCompletedMilestone = true,
                        QuestId = 1,
                        MileStoneIndex = 2
                    },
                    new PlayerQuestState()
                    {
                        PlayerId = "Shround",
                        Id = 3,
                        CreatedDate = DateTime.Now.AddHours(-3),
                        IsCompletedMilestone = true,
                        QuestId = 2,
                        MileStoneIndex = 100
                    }
                }
            });
            context.Players.Add(new Player
            {
                PlayerId = "S1mple",
                PlayerLevel = 4,
                PlayerQuestStates = new List<PlayerQuestState>
                {
                    new PlayerQuestState()
                    {
                        PlayerId = "S1mple",
                        Id = 4,
                        CreatedDate = DateTime.Now.AddHours(-1),
                        IsCompletedMilestone = true,
                        QuestId = 2,
                        MileStoneIndex = 1
                    }
                }
            });
            context.SaveChanges();
        }
    }
}
