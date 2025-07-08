namespace SmallService.Infrastructure.Abstractions.Messaging.MassTransit;

public interface IMassTransit
{
    Task Send<TModel>(string queueName, TModel model, TimeSpan? delay = null) where TModel : class;

    Task Publish<TModel>(TModel model, TimeSpan? delay = null) where TModel : class;
}