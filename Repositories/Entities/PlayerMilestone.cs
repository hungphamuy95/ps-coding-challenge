using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Repositories.Entities
{
    public class PlayerMilestone
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public int MilestoneIndex { get; set; }
        public DateTime CreateDate { get; set; }
        public int ChipsAwarded { get; set; }
    }
}
