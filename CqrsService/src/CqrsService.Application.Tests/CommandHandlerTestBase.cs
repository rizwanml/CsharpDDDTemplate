using AutoMapper;
using CqrsService.Application.Configuration;

namespace CqrsService.Application.Tests;

public class CommandHandlerTestBase
{
    public IMapper _mapper;

    public CommandHandlerTestBase()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ApplicationAutoMapperProfile>();
        });
        _mapper = config.CreateMapper();
    }
}