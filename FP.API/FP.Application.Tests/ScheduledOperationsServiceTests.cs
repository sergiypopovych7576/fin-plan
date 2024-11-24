using FP.Application.Interfaces;
using FP.Application.Services;
using FP.Domain;
using FP.Domain.Enums;
using MockQueryable;
using Moq;

namespace FP.Application.Tests
{
    [TestClass]
    public class ScheduledOperationsServiceTests
    {
        private Mock<IRepository<ScheduledOperation>> _scheduledOperationRepoMock;

        private ScheduledOperationsService _scheduledOperaions;

        public ScheduledOperationsServiceTests()
        {
            _scheduledOperationRepoMock = new Mock<IRepository<ScheduledOperation>>();

            _scheduledOperaions = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
        }

        [TestMethod]
        public async Task GetPlannedScheduledOperationsByDateRange_ShouldReturnOperations_Monthly()
        {
            var startDate = new DateOnly(2020, 3, 1);
            var endDate = new DateOnly(2020, 3, 31);

            var salarySchedId = Guid.NewGuid();
            var rentSchedId = Guid.NewGuid();
            var bimonthlySchedId = Guid.NewGuid();
            var scheduledOperations = new List<ScheduledOperation> {
                new ScheduledOperation   {
                    Id = salarySchedId,
                    Amount = 1000,
                    Name = "Monthl Income",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 1),
                },
                 new ScheduledOperation {
                     Id = rentSchedId,
                    Amount = 200,
                    Name = "Monthly Exepense",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 20),
                },
                 new ScheduledOperation {
                     Id = bimonthlySchedId,
                    Amount = 70,
                    Name = "Bimonthly",
                    Type = OperationType.Expense,
                    Interval = 2,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 7),
                },
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());


            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);

            // Act
            var result = await _scheduledOperaions.GetPlannedScheduledOperationsByDateRange(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 1000 && c.Date == new DateOnly(2020, 3, 1) && c.ScheduledOperationId == salarySchedId));
            Assert.IsTrue(result.Any(c => c.Amount == 200 && c.Date == new DateOnly(2020, 3, 20) && c.ScheduledOperationId == rentSchedId));
            Assert.IsTrue(result.Any(c => c.Amount == 70 && c.Date == new DateOnly(2020, 3, 7) && c.ScheduledOperationId == bimonthlySchedId));
        }

        [TestMethod]
        public async Task GetPlannedScheduledOperationsByDateRange_ShouldReturnOperations_Yearly()
        {
            var startDate = new DateOnly(2020, 3, 1);
            var endDate = new DateOnly(2020, 3, 31);

            var salarySchedId = Guid.NewGuid();
            var yearlySchedId = Guid.NewGuid();
            var scheduledOperations = new List<ScheduledOperation> {
                  new ScheduledOperation {
                     Id = yearlySchedId,
                    Amount = 100,
                    Name = "Yearly",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Yearly,
                    StartDate = new DateOnly(2019, 3, 5),
                }
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());


            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);

            // Act
            var result = await _scheduledOperaions.GetPlannedScheduledOperationsByDateRange(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count); ;
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 3, 5) && c.ScheduledOperationId == yearlySchedId));
        }

        [TestMethod]
        public async Task GetPlannedScheduledOperationsByDateRange_ShouldReturnOperations_Expired()
        {
            var startDate = new DateOnly(2020, 3, 1);
            var endDate = new DateOnly(2020, 3, 31);

            var scheduledOperations = new List<ScheduledOperation> {
                new ScheduledOperation   {
                    Amount = 600,
                    Name = "Ended",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2019, 1, 1),
                    EndDate = new DateOnly(2020, 1, 1)
                },
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());


            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);

            // Act
            var result = await _scheduledOperaions.GetPlannedScheduledOperationsByDateRange(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetPlannedScheduledOperationsByDateRange_ShouldReturnOperations_JustStarted()
        {
            var startDate = new DateOnly(2020, 3, 1);
            var endDate = new DateOnly(2020, 3, 31);

            var justStartedSchedId = Guid.NewGuid();
            var justStartedSecondSchedId = Guid.NewGuid();
            var scheduledOperations = new List<ScheduledOperation> {
                 new ScheduledOperation   {
                    Id = justStartedSchedId,
                    Amount = 25,
                    Name = "JustStarted",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 3, 17),
                },
                  new ScheduledOperation   {
                    Id = justStartedSecondSchedId,
                    Amount = 30,
                    Name = "JustStartedSecond",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 3, 1),
                },
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());


            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);

            // Act
            var result = await _scheduledOperaions.GetPlannedScheduledOperationsByDateRange(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 25 && c.Date == new DateOnly(2020, 3, 17) && c.ScheduledOperationId == justStartedSchedId));
            Assert.IsTrue(result.Any(c => c.Amount == 30 && c.Date == new DateOnly(2020, 3, 1) && c.ScheduledOperationId == justStartedSecondSchedId));
        }
    }
}
