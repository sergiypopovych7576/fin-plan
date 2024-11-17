using AutoMapper;
using FP.Application.DTOs;
using FP.Domain;

namespace FP.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Operation, OperationDto>().ReverseMap();
            CreateMap<ScheduledOperation, OperationDto>().ReverseMap();
            CreateMap<Account, AccountDto>().ReverseMap();
        }
    }
}
