namespace CqrsService.Infrastructure.Provider.Messaging.MassTransit;

public interface IMassTransitProvider
{
    Task Send<TModel>(string queueName, TModel model, TimeSpan? delay = null) where TModel : class;

    Task Publish<TModel>(TModel model, TimeSpan? delay = null) where TModel : class;
}