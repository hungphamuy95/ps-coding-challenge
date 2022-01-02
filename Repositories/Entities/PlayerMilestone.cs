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
        [MaxLength(250)]
        [Required]
        public string PlayerId { get; set; }
        [Required]
        public int MilestoneIndex { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public int ChipsAwarded { get; set; }
    }
}
