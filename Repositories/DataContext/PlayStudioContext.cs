using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;

namespace Repositories.DataContext
{
    public class PlayStudioContext : DbContext
    {
        public PlayStudioContext(DbContextOptions<PlayStudioContext> options) : base(options)
        {
                
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerQuestState> PlayerQuestStates { get; set; }
    }
}
