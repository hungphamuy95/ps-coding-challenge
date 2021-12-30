using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ProgressRequestModel
    {
        [Required]
        public string PlayerId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int PlayerLevel { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ChipAmountBet { get; set; }
    }
}
