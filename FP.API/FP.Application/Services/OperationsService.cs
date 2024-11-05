using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IOperationsService : IBaseService
    {
        Task<List<OperationDto>> GetMonthlyOperations(int year, int month);
    }

    public class OperationsService : IOperationsService
    {
        private readonly IRepository<Operation> _repo;
        private readonly IMapper _mapper;
        public OperationsService(IRepository<Operation> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<OperationDto>> GetMonthlyOperations(int year, int month)
        {
            return await _mapper.ProjectTo<OperationDto>(_repo.GetAll()
              .Where(o => o.Date.Month == month && o.Date.Year == year))
              .ToListAsync();
        }
    }
}
