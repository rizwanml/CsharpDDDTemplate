using AutoMapper;
using SmallService.Domain.Entities.ExamplePersonModule;
using SmallService.Infrastructure.Providers.ReqRes.Models;
using SmallService.Infrastructure.Repositories.Entities;

namespace SmallService.Infrastructure.Configuration;

public sealed class InfrastructureAutoMapperProfile : Profile
{
    public InfrastructureAutoMapperProfile()
    {
        CreateMap<ExamplePerson, ExamplePersonDbEntity>().ReverseMap();
        CreateMap<ExamplePerson, CreateExamplePersonRequest>().ReverseMap();
        CreateMap<ExamplePerson, ExamplePersonModel>().ReverseMap();
    }
}