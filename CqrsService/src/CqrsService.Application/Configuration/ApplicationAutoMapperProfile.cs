using AutoMapper;
using CqrsService.Application.Commands;
using CqrsService.Domain.Entities.ExamplePersonModule;

namespace CqrsService.Application.Configuration;

public sealed class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        CreateMap<CreateExamplePersonCommand, ExamplePerson>();
        CreateMap<UpdateExamplePersonCommand, ExamplePerson>();
    }
}