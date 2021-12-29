using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repositories.Entities
{
    public class PlayerQuestState
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public int QuestId { get; set; }
        public int MileStoneIndex { get; set; }
        public bool IsCompletedMilestone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
