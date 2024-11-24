using AutoMapper;
using FP.Application.Interfaces;
using FP.Application.Mapping;
using FP.Application.Services;
using FP.Domain;
using FP.Domain.Enums;
using MockQueryable;
using Moq;

namespace FP.Application.Tests
{
    [TestClass]
    public class OperationsServiceTests
    {
        private Mock<IRepository<Operation>> _operationRepoMock;
        private Mock<IScheduledOperationsService> _scheduledOperationsServiceMock;
        private Mock<IAccountService> _accountServiceMock;
        private IMapper _mapper;

        private OperationsService _operationsService;

        public OperationsServiceTests()
        {
            _operationRepoMock = new Mock<IRepository<Operation>>();
            _scheduledOperationsServiceMock = new Mock<IScheduledOperationsService>();
            _accountServiceMock = new Mock<IAccountService>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _operationsService = new OperationsService(_operationRepoMock.Object, _accountServiceMock.Object, _scheduledOperationsServiceMock.Object, _mapper);
        }

        [TestMethod]
        public async Task GetMonthlyOperations_ShouldReturnOperations()
        {
            var targetDate = new DateOnly(2020, 5, 31);
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 1000,
                IsDefault = true
            };
            var secondAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100,
            };
            _accountServiceMock.Setup(r => r.GetAccounts()).Returns(Task.FromResult(new List<Account> { defaultAccount, secondAccount }));

            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Past Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 4, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 5, 5),
                    Type = OperationType.Expense,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(It.IsAny<Guid>(), targetDate)).Returns(Task.FromResult(new List<Operation>()));

            var result = await _operationsService.GetMonthlyOperations(targetDate, CancellationToken.None);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 1000 && c.Date == new DateOnly(2020, 5, 1) && c.Name == "Salary"));
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 5, 5) && c.Name == "Groceries"));
        }

        [TestMethod]
        public async Task GetMonthlyOperations_ShouldReturnOperations_WithAppliedScheduled()
        {
            var targetDate = new DateOnly(2020, 5, 31);
            var salarySchedId = Guid.NewGuid();
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 1000,
                IsDefault = true
            };
            var secondAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 100,
            };
            _accountServiceMock.Setup(r => r.GetAccounts()).Returns(Task.FromResult(new List<Account> { defaultAccount, secondAccount }));

            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Past Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 4, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = true,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 5, 5),
                    Type = OperationType.Expense,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation> {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = false,
                    ScheduledOperationId = salarySchedId
                },
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(It.IsAny<Guid>(), targetDate)).Returns(Task.FromResult(scheduledOperations));

            var result = await _operationsService.GetMonthlyOperations(targetDate, CancellationToken.None);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 1000 && c.Date == new DateOnly(2020, 5, 1) && c.Name == "Salary"));
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 5, 5) && c.Name == "Groceries"));
        }

        [TestMethod]
        public async Task GetMonthlyOperations_ShouldReturnOperations_WithNotAppliedScheduled()
        {
            var targetDate = new DateOnly(2020, 5, 31);
            var salarySchedId = Guid.NewGuid();
            var billsSchedId = Guid.NewGuid();
            var defaulAccountId = Guid.NewGuid();
            var secondAccountId = Guid.NewGuid();
            var defaultAccount = new Account
            {
                Id = defaulAccountId,
                Balance = 1000,
                IsDefault = true
            };
            var secondAccount = new Account
            {
                Id = secondAccountId,
                Balance = 100,
            };
            _accountServiceMock.Setup(r => r.GetAccounts()).Returns(Task.FromResult(new List<Account> { defaultAccount, secondAccount }));

            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Past Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 4, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = true,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 5, 5),
                    Type = OperationType.Expense,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation> {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = false,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Bills",
                    Amount = 200,
                    Date = new DateOnly(2020, 5, 15),
                    Type = OperationType.Expense,
                    Applied = false,
                    ScheduledOperationId = billsSchedId
                },
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(defaulAccountId, targetDate)).Returns(Task.FromResult(scheduledOperations));
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(secondAccountId, targetDate)).Returns(Task.FromResult(new List<Operation>()));

            var result = await _operationsService.GetMonthlyOperations(targetDate, CancellationToken.None);

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 1000 && c.Date == new DateOnly(2020, 5, 1) && c.Name == "Salary"));
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 5, 5) && c.Name == "Groceries"));
            Assert.IsTrue(result.Any(c => c.Amount == 200 && c.Date == new DateOnly(2020, 5, 15) && c.Name == "Bills"));
        }

        [TestMethod]
        public async Task GetMonthlyOperations_ShouldReturnOperations_With2ndAccount()
        {
            var targetDate = new DateOnly(2020, 5, 31);
            var salarySchedId = Guid.NewGuid();
            var billsSchedId = Guid.NewGuid();
            var secondAccSchedId = Guid.NewGuid();
            var defaulAccountId = Guid.NewGuid();
            var secondAccountId = Guid.NewGuid();
            var defaultAccount = new Account
            {
                Id = defaulAccountId,
                Balance = 1000,
                IsDefault = true
            };
            var secondAccount = new Account
            {
                Id = secondAccountId,
                Balance = 100,
            };
            _accountServiceMock.Setup(r => r.GetAccounts()).Returns(Task.FromResult(new List<Account> { defaultAccount, secondAccount }));

            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Past Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 4, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = true,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 5, 5),
                    Type = OperationType.Expense,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation> {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 5, 1),
                    Type = OperationType.Income,
                    Applied = false,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Bills",
                    Amount = 200,
                    Date = new DateOnly(2020, 5, 15),
                    Type = OperationType.Expense,
                    Applied = false,
                    ScheduledOperationId = billsSchedId
                },
            };
            var secondAccScheduledOperations = new List<Operation>
            {
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Second acc",
                    Amount = 100,
                    Date = new DateOnly(2020, 5, 25),
                    Type = OperationType.Expense,
                    Applied = false,
                    ScheduledOperationId = secondAccSchedId
                }
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(defaulAccountId, targetDate)).Returns(Task.FromResult(scheduledOperations));
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsForMonth(secondAccountId, targetDate)).Returns(Task.FromResult(secondAccScheduledOperations));

            var result = await _operationsService.GetMonthlyOperations(targetDate, CancellationToken.None);

            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(c => c.Amount == 1000 && c.Date == new DateOnly(2020, 5, 1) && c.Name == "Salary"));
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 5, 5) && c.Name == "Groceries"));
            Assert.IsTrue(result.Any(c => c.Amount == 200 && c.Date == new DateOnly(2020, 5, 15) && c.Name == "Bills"));
            Assert.IsTrue(result.Any(c => c.Amount == 100 && c.Date == new DateOnly(2020, 5, 25) && c.Name == "Second acc"));
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 2, 28);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(new List<Operation>()));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2020, result[0].Year);
            Assert.AreEqual(2, result[0].Month);
            Assert.AreEqual(1000, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(900, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary_ForRange()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 3, 31);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next",
                    Amount = 1000,
                    Date = new DateOnly(2020, 3, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next",
                    Amount = 100,
                    Date = new DateOnly(2020, 3, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(new List<Operation>()));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2020, result[0].Year);
            Assert.AreEqual(2, result[0].Month);
            Assert.AreEqual(1000, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(900, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);

            Assert.AreEqual(2020, result[1].Year);
            Assert.AreEqual(3, result[1].Month);
            Assert.AreEqual(1000, result[1].TotalIncomes);
            Assert.AreEqual(100, result[1].TotalExpenses);
            Assert.AreEqual(900, result[1].MonthBalance);
            Assert.AreEqual(1000, result[1].Categories[0].Amount);
            Assert.AreEqual(100, result[1].Categories[1].Amount);
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary_ShouldExcludeTransferFromStatistic()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 2, 28);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Transfer",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Transfer,
                    Category = groceriesCategory,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(new List<Operation>()));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1000, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(900, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary_WithNotAppliedScheduledOperations()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var scheduledCategory = new Category
            {
                Name = "Scheduled"
            };
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 2, 28);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },

            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Scheduled",
                    Amount = 200,
                    Date = new DateOnly(2020, 2, 15),
                    Type = OperationType.Income,
                    Applied = false,
                    Category = scheduledCategory
                },
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(scheduledOperations));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1200, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(1100, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);
            Assert.AreEqual(200, result[0].Categories[2].Amount);
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary_WithAppliedScheduledOperations()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var scheduledCategory = new Category
            {
                Name = "Scheduled"
            };
            var scheduledOpId = Guid.NewGuid();
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 2, 28);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Scheduled",
                    Amount = 200,
                    Date = new DateOnly(2020, 2, 15),
                    Type = OperationType.Income,
                    Category = scheduledCategory,
                    ScheduledOperationId = scheduledOpId,
                    Applied = true
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Scheduled",
                    Amount = 200,
                    Date = new DateOnly(2020, 2, 15),
                    Type = OperationType.Income,
                    Applied = false,
                    Category = scheduledCategory,
                    ScheduledOperationId = scheduledOpId
                },
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(scheduledOperations));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1200, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(1100, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);
            Assert.AreEqual(200, result[0].Categories[2].Amount);
        }

        [TestMethod]
        public async Task GetSummaryByDateRange_ShouldReturnMonthSummary_WithTransferSchedule()
        {
            var salaryCategory = new Category
            {
                Name = "Salary"
            };
            var groceriesCategory = new Category
            {
                Name = "Groceries"
            };
            var scheduledCategory = new Category
            {
                Name = "Scheduled"
            };
            var startDate = new DateOnly(2020, 2, 1);
            var endDate = new DateOnly(2020, 2, 28);
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 1),
                    Type = OperationType.Income,
                    Category = salaryCategory,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 5),
                    Type = OperationType.Expense,
                    Category = groceriesCategory,
                    Applied = true
                },

            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Scheduled",
                    Amount = 200,
                    Date = new DateOnly(2020, 2, 15),
                    Type = OperationType.Transfer,
                    Applied = false,
                    Category = scheduledCategory
                },
            };
            _scheduledOperationsServiceMock.Setup(r => r.GetPlannedScheduledOperationsByDateRange(startDate, endDate)).Returns(Task.FromResult(scheduledOperations));

            var result = await _operationsService.GetSummaryByDateRange(startDate, endDate, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1000, result[0].TotalIncomes);
            Assert.AreEqual(100, result[0].TotalExpenses);
            Assert.AreEqual(900, result[0].MonthBalance);
            Assert.AreEqual(1000, result[0].Categories[0].Amount);
            Assert.AreEqual(100, result[0].Categories[1].Amount);
        }
    }
}
