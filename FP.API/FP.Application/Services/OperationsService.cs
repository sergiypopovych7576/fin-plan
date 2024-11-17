using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IOperationsService : IBaseService
    {
        Task<List<OperationDto>> GetMonthlyOperations(DateOnly date, CancellationToken cancellationToken);
        Task Create(OperationDto operation);
        Task Delete(Guid id);
        Task Sync();
    }

    public class OperationsService : IOperationsService
    {
        private readonly IRepository<Operation> _repo;
        private readonly IScheduledOperationsService _scheduledOperationsService;
        private readonly IAccountService _accService;
        private readonly IMapper _mapper;

        public OperationsService(IRepository<Operation> repo, IAccountService accService,
            IScheduledOperationsService scheduledOperationsService,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _scheduledOperationsService = scheduledOperationsService;
            _accService = accService;
        }

        public async Task Delete(Guid id)
        {
            var operation = await _repo.GetByIdAsync(id);
            _repo.Remove(operation);
            if (!operation.Applied)
            {
                await _repo.SaveChangesAsync();
                return;
            }
            await _accService.RemoveOperation(operation);
        }

        public async Task Create(OperationDto operation)
        {
            if (operation.Frequency != null)
            {
                await _scheduledOperationsService.Create(_mapper.Map<ScheduledOperation>(operation));
                return;
            }
            var mappedOp = _mapper.Map<Operation>(operation);
            if (operation.Date > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                await _repo.AddAsync(mappedOp);
                await _repo.SaveChangesAsync();
                return;
            }
            mappedOp.Applied = true;
            await _repo.AddAsync(mappedOp);
            await _accService.ApplyOperation(mappedOp);
        }

        public async Task Sync()
        {
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var notAppliedOperations = await _repo.GetAll()
                .Where(o => !o.Applied && o.Date <= now)
                .ToListAsync();

            var firstNotAppliedDate = notAppliedOperations.OrderBy(c => c.Date).FirstOrDefault();
            var appliedScheduleOperations = _repo.GetAll().Where(c => c.Date <= now
                && c.Applied && c.ScheduledOperationId != null);
            if (firstNotAppliedDate != null)
            {
                appliedScheduleOperations = appliedScheduleOperations.Where(c => c.Date >= firstNotAppliedDate.Date);
            }
            var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsUpToMonth(now);

            var filteredScheduledOperations = scheduledOperations
                .Where(scheduled => !appliedScheduleOperations.Any(operation =>
                    operation.ScheduledOperationId == scheduled.ScheduledOperationId && operation.Date == scheduled.Date
                        && operation.Amount == scheduled.Amount))
                .ToList();

            var operationsToApply = notAppliedOperations.Concat(filteredScheduledOperations)
                .ToList();
            operationsToApply.ForEach(c => c.Applied = true);
            await _repo.AddRangeAsync(filteredScheduledOperations);
            _repo.Update(notAppliedOperations);
            await _accService.ApplyOperations(operationsToApply);
        }

        public async Task<List<OperationDto>> GetMonthlyOperations(DateOnly date, CancellationToken cancellationToken)
        {
            var operations = await _mapper.ProjectTo<OperationDto>(
                _repo.GetAll()
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Where(o => o.Date.Year == date.Year && o.Date.Month == date.Month)
                    .OrderBy(c => c.Date))
                .ToListAsync(cancellationToken);

            var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsForMonth(date);

            var filteredScheduledOperations = scheduledOperations
                .Where(scheduled => !operations.Any(operation =>
                    operation.ScheduledOperationId == scheduled.ScheduledOperationId && operation.Applied && operation.Date == scheduled.Date
                        && operation.Amount == scheduled.Amount))
                .ToList();

            operations.AddRange(_mapper.Map<List<OperationDto>>(filteredScheduledOperations));

            return operations.OrderBy(o => o.Date).ThenBy(c => c.Type).ToList();
        }
    }
}
