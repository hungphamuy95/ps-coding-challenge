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
    class StateServiceTest
    {
        private Mock<IBaseRepository<PlayerQuest>> _playerQuestRepositoryMock;
        private Mock<IBaseRepository<PlayerMilestone>> _playerMilestoneRepositoryMock;
        private Mock<IQuestLoader> _questLoaderMock;
        private Mock<ILogger<StateService>> _loggerMock;
        private StateService _stateService;

        [SetUp]
        public void Setup()
        {
            _playerQuestRepositoryMock = new Mock<IBaseRepository<PlayerQuest>>();
            _playerMilestoneRepositoryMock = new Mock<IBaseRepository<PlayerMilestone>>();
            _questLoaderMock = new Mock<IQuestLoader>();
            _loggerMock = new Mock<ILogger<StateService>>();
            _stateService = new StateService(_playerQuestRepositoryMock.Object, _playerMilestoneRepositoryMock.Object,
                _loggerMock.Object, _questLoaderMock.Object);
        }

        [Test]
        public void If_State_service_throws_exception()
        {
            _playerQuestRepositoryMock.Setup(x => x.Count(It.IsAny<Expression<Func<PlayerQuest, bool>>>())).Throws(new Exception());
            var res = (_stateService.GetState("abc")).Result;
            Assert.IsNull(res);
        }
        [Test]
        public void If_state_service_return_result()
        {
            _questLoaderMock.Setup(x => x.GetAllQuest()).Returns(GetAllQuestsMockOne());
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
                }
            });
            _playerMilestoneRepositoryMock.Setup(x => x.GetAsQueryable()).Returns(queryPlayerMilestone);
            var res = (_stateService.GetState("abc")).Result;
            var expectedRes = new StateResponseModel
            {
                TotalQuestPercentCompleted = 50,
                LastMilestoneIndexCompleted = 3
            };
            Assert.AreEqual(JsonSerializer.Serialize(res), JsonSerializer.Serialize(expectedRes));
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
    }
}
