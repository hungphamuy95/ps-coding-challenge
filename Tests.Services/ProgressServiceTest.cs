using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using NUnit.Framework;
using Repositories;
using Repositories.Entities;
using Repositories.JsonLoader;
using Services.Implements;

namespace Tests.Services
{
    [TestFixture]
    class ProgressServiceTest
    {
        private  Mock<IBaseRepository<PlayerQuest>> _playerQuestRepositoryMock;
        private  Mock<IBaseRepository<PlayerMilestone>> _playerMilestoneRepositoryMock;
        private  Mock<IQuestLoader> _questLoaderMock;
        private  IOptions<BetValueConfig> _settingMock;
        private Mock<ILogger<ProgressService>> _loggerMock;
        private ProgressService _progressService;
        [SetUp]
        public void Setup()
        {
            _playerQuestRepositoryMock = new Mock<IBaseRepository<PlayerQuest>>();
            _playerMilestoneRepositoryMock = new Mock<IBaseRepository<PlayerMilestone>>();
            _questLoaderMock = new Mock<IQuestLoader>();
            _settingMock = Options.Create<BetValueConfig>(new BetValueConfig
            {
                RateFromBet = 3,
                LevelBonusRate = 3
            });
            _loggerMock = new Mock<ILogger<ProgressService>>();
            _progressService = new ProgressService(_playerQuestRepositoryMock.Object,
                _playerMilestoneRepositoryMock.Object, _questLoaderMock.Object, _settingMock, _loggerMock.Object);
        }

        [Test]
        public void If_Service_Throw_Exception()
        {
            _questLoaderMock.Setup(x => x.GetAllQuest()).Returns(GetAllQuestsMockOne());
            _playerQuestRepositoryMock.Setup(x => x.Add(new PlayerQuest
            {
                PlayerId = "test",
                QuestId = 1,
                CreatedDate = DateTime.Now,
                Id = Guid.NewGuid()
            })).Throws(new Exception());
            var res = _progressService.Process(RequestMock("abc", 1, 1));
            Assert.IsNull(res.Result);
        }

        [Test]
        public void If_Player_does_not_complete_any_quest()
        {
            _questLoaderMock.Setup(x => x.GetAllQuest()).Returns(GetAllQuestsMockOne());
            _playerQuestRepositoryMock.Setup(x => x.Count(z => z.PlayerId == "")).ReturnsAsync(0);
            var queryPlayerMileStone = new TestAsyncEnumerable<PlayerMilestone>(new List<PlayerMilestone>()).AsQueryable();
            _playerMilestoneRepositoryMock.Setup(x => x.GetAsQueryable()).Returns(queryPlayerMileStone);

            var res = (_progressService.Process(RequestMock("abc", 1, 1))).Result;
            var expectedRes = new ProgressResponseModel
            {
                TotalQuestPercentCompleted = 0,
                MilestonesCompleted = Array.Empty<MileStoneCompletedModel>(),
                QuestPointsEarned = 6
            };
            Assert.AreEqual(JsonSerializer.Serialize(res), JsonSerializer.Serialize(expectedRes));
        }

        [Test]
        public void If_Player_complete_at_least_milestone_and_quest()
        {
            _questLoaderMock.Setup(x => x.GetAllQuest()).Returns(GetAllQuestsMockTwo());


            var queryPlayerQuest = new TestAsyncEnumerable<PlayerQuest>(new List<PlayerQuest>
            {
                new PlayerQuest
                {
                    PlayerId = "abc",
                    CreatedDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    QuestId = 1
                }
            });
            _playerQuestRepositoryMock.Setup(x => x.Count(It.IsAny<Expression<Func<PlayerQuest, bool>>>()))
                .ReturnsAsync(
                    (Expression<Func<PlayerQuest, bool>> expr) => queryPlayerQuest.Count(expr.Compile())
                );
            var queryPlayerMileStone = new TestAsyncEnumerable<PlayerMilestone>(new List<PlayerMilestone>
            {
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 1,
                    ChipsAwarded = 5,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                }
            }).AsQueryable();

            _playerMilestoneRepositoryMock.Setup(x => x.GetAsQueryable()).Returns(queryPlayerMileStone);

            var res = (_progressService.Process(RequestMock("abc", 1, 1))).Result;
            var expectedRes = new ProgressResponseModel
            {
                TotalQuestPercentCompleted = 50,
                MilestonesCompleted = new MileStoneCompletedModel[]
                {
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 5,
                        MilestoneIndex = 1
                    }
                },
                QuestPointsEarned = 11
            };
            Assert.AreEqual(JsonSerializer.Serialize(res), JsonSerializer.Serialize(expectedRes));
        }

        [Test]
        public void If_user_already_completed_quest_then_attempt_again()
        {
            _questLoaderMock.Setup(x => x.GetAllQuest()).Returns(GetAllQuestsMockThree);
            var queryPlayerQuestCheckExist = new TestAsyncEnumerable<PlayerQuest>(new List<PlayerQuest>
            {
                new PlayerQuest
                {
                    PlayerId = "abc",
                    CreatedDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    QuestId = 1
                }
            });
            var queryPlayerQuestCount = new TestAsyncEnumerable<PlayerQuest>(new List<PlayerQuest>
            {
                new PlayerQuest
                {
                    PlayerId = "abc",
                    CreatedDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    QuestId = 1
                },
                new PlayerQuest
                {
                    PlayerId = "abc",
                    CreatedDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    QuestId = 2
                }
            });
            _playerQuestRepositoryMock.Setup(x=>x.Exist(It.IsAny<Expression<Func<PlayerQuest, bool>>>()))
                .ReturnsAsync(
                (Expression<Func<PlayerQuest, bool>> expr) => queryPlayerQuestCheckExist.Any(expr.Compile())
                );
            _playerQuestRepositoryMock.Setup(x => x.Count(It.IsAny<Expression<Func<PlayerQuest, bool>>>()))
                .ReturnsAsync(
                    (Expression<Func<PlayerQuest, bool>> expr) => queryPlayerQuestCount.Count(expr.Compile())
                );

            var queryPlayerMilestoneCheckExist = new TestAsyncEnumerable<PlayerMilestone>(new List<PlayerMilestone>
            {
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 1,
                    ChipsAwarded = 5,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 2,
                    ChipsAwarded = 6,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 3,
                    ChipsAwarded = 6,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 4,
                    ChipsAwarded = 1,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                }
            });
            var queryPlayerMilestone = new TestAsyncEnumerable<PlayerMilestone>(new List<PlayerMilestone>
            {
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 1,
                    ChipsAwarded = 5,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 2,
                    ChipsAwarded = 6,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 3,
                    ChipsAwarded = 6,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 4,
                    ChipsAwarded = 1,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                new PlayerMilestone
                {
                    PlayerId = "abc",
                    MilestoneIndex = 5,
                    ChipsAwarded = 1,
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid()
                }
            });
            _playerMilestoneRepositoryMock.Setup(x => x.Exist(It.IsAny<Expression<Func<PlayerMilestone, bool>>>()))
                .ReturnsAsync(
                    (Expression<Func<PlayerMilestone, bool>> expr) => queryPlayerMilestoneCheckExist.Any(expr.Compile())
                );
            _playerMilestoneRepositoryMock.Setup(x => x.GetAsQueryable()).Returns(queryPlayerMilestone);
            var res = (_progressService.Process(RequestMock("abc", 1, 1))).Result;
            var expectedRes = new ProgressResponseModel
            {
                TotalQuestPercentCompleted = 100,
                MilestonesCompleted = new MileStoneCompletedModel[]
                {
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 5,
                        MilestoneIndex = 1
                    },
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 6,
                        MilestoneIndex = 2
                    },
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 6,
                        MilestoneIndex = 3
                    },
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 1,
                        MilestoneIndex = 4
                    },
                    new MileStoneCompletedModel
                    {
                        ChipsAwarded = 1,
                        MilestoneIndex = 5
                    }
                },
                QuestPointsEarned = 11
            };
            Assert.AreEqual(JsonSerializer.Serialize(res), JsonSerializer.Serialize(expectedRes));
        }
        

        private static ProgressRequestModel RequestMock(string playerId, int playerLevel, int chipAmountBet)
        {
            return new ProgressRequestModel
            {
                PlayerId = playerId,
                PlayerLevel = playerLevel,
                ChipAmountBet = chipAmountBet
            };
        }

      

        private static IEnumerable<QuestModel> GetAllQuestsMockOne()
        {
            return new List<QuestModel>
            {
                new QuestModel
                {
                    QuestID = 1,
                    QuestName = "test1",
                    PassingPoint = 100,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 1,
                            AwardChip = 5,
                            GoalPoint = 500,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 2,
                            AwardChip = 6,
                            GoalPoint = 100,
                            Order = 2
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 3,
                            AwardChip = 6,
                            GoalPoint = 100,
                            Order = 2
                        }
                    }
                },
                new QuestModel
                {
                    QuestID = 2,
                    QuestName = "test2",
                    PassingPoint = 200,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 4,
                            AwardChip = 4,
                            GoalPoint = 100,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 5,
                            AwardChip = 5,
                            GoalPoint = 100,
                            Order = 1
                        }
                    }
                }
            };
        }

        private static IEnumerable<QuestModel> GetAllQuestsMockTwo()
        {
            return new List<QuestModel>
            {
                new QuestModel
                {
                    QuestID = 1,
                    QuestName = "test1",
                    PassingPoint = 10,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 1,
                            AwardChip = 5,
                            GoalPoint = 5,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 2,
                            AwardChip = 6,
                            GoalPoint = 100,
                            Order = 2
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 3,
                            AwardChip = 6,
                            GoalPoint = 100,
                            Order = 2
                        }
                    }
                },
                new QuestModel
                {
                    QuestID = 2,
                    QuestName = "test2",
                    PassingPoint = 200,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 4,
                            AwardChip = 4,
                            GoalPoint = 100,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 5,
                            AwardChip = 5,
                            GoalPoint = 100,
                            Order = 1
                        }
                    }
                }
            };
        }

        private static IEnumerable<QuestModel> GetAllQuestsMockThree()
        {
            return new List<QuestModel>
            {
                new QuestModel
                {
                    QuestID = 1,
                    QuestName = "test1",
                    PassingPoint = 10,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 1,
                            AwardChip = 5,
                            GoalPoint = 1,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 2,
                            AwardChip = 6,
                            GoalPoint = 1,
                            Order = 2
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 3,
                            AwardChip = 6,
                            GoalPoint = 1,
                            Order = 2
                        }
                    }
                },
                new QuestModel
                {
                    QuestID = 2,
                    QuestName = "test2",
                    PassingPoint = 9,
                    Milestones = new List<MilestoneModel>
                    {
                        new MilestoneModel
                        {
                            MilestoneIndex = 4,
                            AwardChip = 4,
                            GoalPoint = 1,
                            Order = 1
                        },
                        new MilestoneModel
                        {
                            MilestoneIndex = 5,
                            AwardChip = 5,
                            GoalPoint = 1,
                            Order = 1
                        }
                    }
                }
            };
        }
    }
}
