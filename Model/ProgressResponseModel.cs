using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProgressResponseModel
    {
        public int QuestPointsEarned { get; set; }
        public int TotalQuestPercentCompleted { get; set; }
        public MileStoneCompletedModel [] MilestonesCompleted { get; set; }
    }

    public class MileStoneCompletedModel
    {
        public int MilestoneIndex { get; set; }
        public int ChipsAwarded { get; set; }
    }
}
