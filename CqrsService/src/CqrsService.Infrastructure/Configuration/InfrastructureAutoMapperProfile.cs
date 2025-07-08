using AutoMapper;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Infrastructure.Persistence.Entities;
using CqrsService.Infrastructure.Provider.External.ReqRes.Models;

namespace CqrsService.Infrastructure.Configuration;

public sealed class InfrastructureAutoMapperProfile : Profile
{
    public InfrastructureAutoMapperProfile()
    {
        CreateMap<ExamplePerson, ExamplePersonDbEntity>().ReverseMap();
        CreateMap<ExamplePerson, CreateExamplePersonRequest>().ReverseMap();
        CreateMap<ExamplePerson, ExamplePersonModel>().ReverseMap();
    }
}