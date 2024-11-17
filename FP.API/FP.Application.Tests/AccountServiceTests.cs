using AutoMapper;
using FP.Application.Interfaces;
using FP.Application.Mapping;
using FP.Application.Services;
using FP.Domain;
using FP.Domain.Enums;
using MockQueryable;
using Moq;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FP.Application.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private Mock<IRepository<Operation>> _operationRepoMock;
        private Mock<IRepository<ScheduledOperation>> _scheduledOperationRepoMock;
        private Mock<IRepository<Account>> _accountRepoMock;
        private Mock<IDateService> _dateServiceMock;
        private IMapper _mapper;

        private AccountService _accountService;

        public AccountServiceTests()
        {
            _operationRepoMock = new Mock<IRepository<Operation>>();
            _scheduledOperationRepoMock = new Mock<IRepository<ScheduledOperation>>();
            _accountRepoMock = new Mock<IRepository<Account>>();
            _dateServiceMock = new Mock<IDateService>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Register the actual profile
            });
            _mapper = mapperConfig.CreateMapper();

            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);

            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                scheduledService,
                _mapper
            );
        }

        [TestMethod]
        public async Task GetBalanceSummary_ShouldReturnData_Operations()
        {
            var date = new DateOnly(2020, 1, 15);
            _dateServiceMock.Setup(c => c.GetUtcDate()).Returns(new DateTime(date, TimeOnly.MinValue));
            // Arrange
            var targetDate = new DateOnly(2020, 1, 31);

            // Mock default account
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 900,
                IsDefault = true
            };
            var accounts = new List<Account>() { defaultAccount };
            var mock = accounts.AsQueryable().BuildMock();
            _accountRepoMock.Setup(r => r.GetAll()).Returns(mock);

            // Mock operations
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 2),
                    Type = OperationType.Expense,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<ScheduledOperation> {
                new ScheduledOperation   {
                    Amount = 500,
                    Name = "Investment",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 17)
                },
                 new ScheduledOperation   {
                    Amount = 200,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 18)
                }
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());


            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                 scheduledService,
                _mapper
            );

            // Act
            var result = await _accountService.GetBalanceSummary(targetDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(900, result.Balance); // Starting balance
            Assert.AreEqual(2500, result.Incomes); // Monthly incomes
            Assert.AreEqual(400, result.Expenses); // Monthly expenses (Groceries + Rent)
            Assert.AreEqual(2100, result.MonthBalance); // Month balance (Incomes - Expenses)
            Assert.AreEqual(2100, result.EndMonthBalance); // End month balance
        }

        [TestMethod]
        public async Task GetBalanceSummary_ShouldReturnData_Operations_WhenLooksIntoNextMonth()
        {
            var date = new DateOnly(2020, 1, 15);
            _dateServiceMock.Setup(c => c.GetUtcDate()).Returns(new DateTime(date, TimeOnly.MinValue));
            // Arrange
            var targetDate = new DateOnly(2020, 2, 29);

            // Mock default account
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 900,
                IsDefault = true
            };
            var accounts = new List<Account>() { defaultAccount };
            var mock = accounts.AsQueryable().BuildMock();
            _accountRepoMock.Setup(r => r.GetAll()).Returns(mock);

            // Mock operations
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next month",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 2),
                    Type = OperationType.Expense,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next month",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<ScheduledOperation> {
               new ScheduledOperation   {
                    Amount = 500,
                    Name = "Investment",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 17)
                },
                 new ScheduledOperation   {
                    Amount = 200,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 18)
                }
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());

            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                 scheduledService,
                _mapper
            );

            // Act
            var result = await _accountService.GetBalanceSummary(targetDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2100, result.Balance); // Starting balance
            Assert.AreEqual(1500, result.Incomes); // Monthly incomes
            Assert.AreEqual(300, result.Expenses); // Monthly expenses (Groceries + Rent)
            Assert.AreEqual(1200, result.MonthBalance); // Month balance (Incomes - Expenses)
            Assert.AreEqual(3300, result.EndMonthBalance); // End month balance
        }

        [TestMethod]
        public async Task GetBalanceSummary_ShouldReturnData_Operations_WhenLooksIntoNextMonth_TwoMonth()
        {
            var date = new DateOnly(2020, 1, 15);
            _dateServiceMock.Setup(c => c.GetUtcDate()).Returns(new DateTime(date, TimeOnly.MinValue));
            // Arrange
            var targetDate = new DateOnly(2020, 3, 29);

            // Mock default account
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 900,
                IsDefault = true
            };
            var accounts = new List<Account>() { defaultAccount };
            var mock = accounts.AsQueryable().BuildMock();
            _accountRepoMock.Setup(r => r.GetAll()).Returns(mock);

            // Mock operations
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 1),
                    Type = OperationType.Income,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next",
                    Amount = 1000,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next month",
                    Amount = 1000,
                    Date = new DateOnly(2020, 2, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                  new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary next next month",
                    Amount = 2000,
                    Date = new DateOnly(2020, 3, 17),
                    Type = OperationType.Income,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 2),
                    Type = OperationType.Expense,
                    Applied = true
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next",
                    Amount = 100,
                    Date = new DateOnly(2020, 1, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next month",
                    Amount = 100,
                    Date = new DateOnly(2020, 2, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Groceries next next month",
                    Amount = 100,
                    Date = new DateOnly(2020, 3, 17),
                    Type = OperationType.Expense,
                    Applied = false
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<ScheduledOperation> {
                 new ScheduledOperation   {
                    Amount = 500,
                    Name = "Investment",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 17)
                },
                 new ScheduledOperation   {
                    Amount = 200,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2020, 1, 18)
                }
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());

            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                 scheduledService,
                _mapper
            );

            // Act
            var result = await _accountService.GetBalanceSummary(targetDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3300, result.Balance); // Starting balance
            Assert.AreEqual(2500, result.Incomes); // Monthly incomes
            Assert.AreEqual(300, result.Expenses); // Monthly expenses (Groceries + Rent)
            Assert.AreEqual(2200, result.MonthBalance); // Month balance (Incomes - Expenses)
            Assert.AreEqual(5500, result.EndMonthBalance); // End month balance
        }

        [TestMethod]
        public async Task GetBalanceSummary_CaseOne_CurrentMonth()
        {
            var date = new DateOnly(2024, 11, 17);
            _dateServiceMock.Setup(c => c.GetUtcDate()).Returns(new DateTime(date, TimeOnly.MinValue));
            // Arrange
            var targetDate = new DateOnly(2024, 11, 30);

            // Mock default account
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 7646,
                IsDefault = true
            };
            var accounts = new List<Account>() { defaultAccount };
            var mock = accounts.AsQueryable().BuildMock();
            _accountRepoMock.Setup(r => r.GetAll()).Returns(mock);

            var salarySchedId = Guid.NewGuid();
            var miscSchedId = Guid.NewGuid();
            var rentSchedId = Guid.NewGuid();
            // Mock operations
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 3650,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Income,
                    Applied = true,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Misc",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Expense,
                    Applied = true,
                    ScheduledOperationId = miscSchedId
                },
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Rent",
                    Amount = 200,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Expense,
                    Applied = true,
                    ScheduledOperationId = rentSchedId
                },
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Alex BD",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 5),
                    Type = OperationType.Expense,
                    Applied = true,
                },
                   new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Vet",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 26),
                    Type = OperationType.Expense,
                    Applied = false,
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<ScheduledOperation> {
                 new ScheduledOperation   {
                     Id = rentSchedId,
                    Amount = 200,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                 new ScheduledOperation   {
                     Id = miscSchedId,
                    Amount = 100,
                    Name = "Misc",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                  new ScheduledOperation   {
                      Id = salarySchedId,
                    Amount = 3650,
                    Name = "Salary",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                   new ScheduledOperation   {
                    Amount = 200,
                    Name = "Language",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                    new ScheduledOperation   {
                    Amount = 12,
                    Name = "Youtube",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                      new ScheduledOperation   {
                    Amount = 350,
                    Name = "Food",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                      new ScheduledOperation   {
                    Amount = 120,
                    Name = "Bills",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                new ScheduledOperation   {
                    Amount = 38,
                    Name = "Internet",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 20)
                },
                         new ScheduledOperation   {
                    Amount = 440,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 20)
                },
                         new ScheduledOperation   {
                    Amount = 80,
                    Name = "Administration",
                    Type = OperationType.Expense,
                    Interval = 2,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 12, 20)
                },
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());

            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                 scheduledService,
                _mapper
            );

            // Act
            var result = await _accountService.GetBalanceSummary(targetDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7646, result.Balance); // Starting balance
            Assert.AreEqual(3650, result.Incomes); // Monthly incomes
            Assert.AreEqual(1660, result.Expenses); // Monthly expenses (Groceries + Rent)
            Assert.AreEqual(1990, result.MonthBalance); // Month balance (Incomes - Expenses)
            Assert.AreEqual(6386, result.EndMonthBalance); // End month balance
        }

        [TestMethod]
        public async Task GetBalanceSummary_CaseOne_NextMonth()
        {
            var date = new DateOnly(2024, 11, 17);
            _dateServiceMock.Setup(c => c.GetUtcDate()).Returns(new DateTime(date, TimeOnly.MinValue));
            // Arrange
            var targetDate = new DateOnly(2024, 12, 31);

            // Mock default account
            var defaultAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 7646,
                IsDefault = true
            };
            var accounts = new List<Account>() { defaultAccount };
            var mock = accounts.AsQueryable().BuildMock();
            _accountRepoMock.Setup(r => r.GetAll()).Returns(mock);

            var salarySchedId = Guid.NewGuid();
            var miscSchedId = Guid.NewGuid();
            var rentSchedId = Guid.NewGuid();
            // Mock operations
            var operations = new List<Operation>
            {
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Salary",
                    Amount = 3650,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Income,
                    Applied = true,
                    ScheduledOperationId = salarySchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Misc",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Expense,
                    Applied = true,
                    ScheduledOperationId = miscSchedId
                },
                 new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Rent",
                    Amount = 200,
                    Date = new DateOnly(2024, 11, 1),
                    Type = OperationType.Expense,
                    Applied = true,
                    ScheduledOperationId = rentSchedId
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Alex BD",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 5),
                    Type = OperationType.Expense,
                    Applied = true,
                },
                new Operation
                {
                    Id = Guid.NewGuid(),
                    Name = "Vet",
                    Amount = 100,
                    Date = new DateOnly(2024, 11, 26),
                    Type = OperationType.Expense,
                    Applied = false,
                },
            };
            _operationRepoMock.Setup(r => r.GetAll()).Returns(operations.AsQueryable().BuildMock());

            var scheduledOperations = new List<ScheduledOperation> {
                 new ScheduledOperation   {
                     Id = rentSchedId,
                    Amount = 200,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                 new ScheduledOperation   {
                     Id = miscSchedId,
                    Amount = 100,
                    Name = "Misc",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                  new ScheduledOperation   {
                      Id = salarySchedId,
                    Amount = 3650,
                    Name = "Salary",
                    Type = OperationType.Income,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 1)
                },
                   new ScheduledOperation   {
                    Amount = 200,
                    Name = "Language",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                    new ScheduledOperation   {
                    Amount = 12,
                    Name = "Youtube",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                      new ScheduledOperation   {
                    Amount = 350,
                    Name = "Food",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                      new ScheduledOperation   {
                    Amount = 120,
                    Name = "Bills",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 18)
                },
                new ScheduledOperation   {
                    Amount = 38,
                    Name = "Internet",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 20)
                },
                         new ScheduledOperation   {
                    Amount = 440,
                    Name = "Rent",
                    Type = OperationType.Expense,
                    Interval = 1,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 11, 20)
                },
                         new ScheduledOperation   {
                    Amount = 80,
                    Name = "Administration",
                    Type = OperationType.Expense,
                    Interval = 2,
                    Frequency = Frequency.Monthly,
                    StartDate = new DateOnly(2024, 12, 20)
                },
            };
            _scheduledOperationRepoMock.Setup(r => r.GetAll()).Returns(scheduledOperations.AsQueryable().BuildMock());

            var scheduledService = new ScheduledOperationsService(_scheduledOperationRepoMock.Object);
            _accountService = new AccountService(
                _accountRepoMock.Object,
                _operationRepoMock.Object,
                _dateServiceMock.Object,
                 scheduledService,
                _mapper
            );

            // Act
            var result = await _accountService.GetBalanceSummary(targetDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6386, result.Balance); // Starting balance
            Assert.AreEqual(3650, result.Incomes); // Monthly incomes
            Assert.AreEqual(1540, result.Expenses); // Monthly expenses (Groceries + Rent)
            Assert.AreEqual(2110, result.MonthBalance); // Month balance (Incomes - Expenses)
            Assert.AreEqual(8496, result.EndMonthBalance); // End month balance
        }
    }
}
