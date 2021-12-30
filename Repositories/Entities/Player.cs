using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repositories.Entities
{
    public class Player
    {
        [Key]
        public string PlayerId { get; set; }
        public int PlayerLevel { get; set; }
        public ICollection<PlayerQuest> PlayerQuests { get; set; }
    }
}
