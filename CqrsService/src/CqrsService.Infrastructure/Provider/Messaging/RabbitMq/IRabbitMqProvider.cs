namespace CqrsService.Infrastructure.Provider.Messaging.RabbitMq;

public interface IRabbitMqProvider
{
    Task Send<TModel>(string queueName, TModel model, Dictionary<string, string> messageAttributes = default) where TModel : class;

    Task<TModel> Receive<TModel>(string queueName) where TModel : class;
}