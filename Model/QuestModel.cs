using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class QuestConfig
    {
        public string Content { get; set; }
    }

    public class QuestModel
    {
        public int QuestID { get; set; }
        public string QuestName { get; set; }
        public int PassingPoint { get; set; }
        public List<MilestoneModel> Milestones { get; set; }
    }

    public class MilestoneModel
    {
        public int MilestoneIndex { get; set; }
        public int AwardChip { get; set; }
        public int GoalPoint { get; set; }
        public int Order { get; set; }
    }
}
