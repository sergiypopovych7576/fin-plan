using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Application.Validations;
using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
	public interface IOperationsService : IBaseService
	{
		Task<List<OperationDto>> GetMonthlyOperations(int year, int month, CancellationToken cancellationToken);
		Task Create(OperationDto operation);
		Task Delete(Guid id);
	}

	public class OperationsService : IOperationsService
	{
		private readonly IRepository<Operation> _repo;
		private readonly IMapper _mapper;
		private readonly CategoryValidator _categoryValidator = new CategoryValidator();

		public OperationsService(IRepository<Operation> repo, IMapper mapper) {
			_repo = repo;
			_mapper = mapper;
		}

		public async Task Delete(Guid id) {
			_repo.Remove(new Operation { Id = id });
			await _repo.SaveChangesAsync();
		}

		public async Task Create(OperationDto operation) {
			await _repo.AddAsync(_mapper.Map<Operation>(operation));
			await _repo.SaveChangesAsync();
		}

		public async Task<List<OperationDto>> GetMonthlyOperations(int year, int month, CancellationToken cancellationToken) {
			return await _mapper.ProjectTo<OperationDto>(_repo.GetAll().Include(c => c.Category)
			  .Where(o => o.Date.Month == month && o.Date.Year == year))
			  .ToListAsync(cancellationToken);
		}
	}
}
