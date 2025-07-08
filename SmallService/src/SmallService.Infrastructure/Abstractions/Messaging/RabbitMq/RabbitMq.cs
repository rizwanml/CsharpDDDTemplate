using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmallService.Infrastructure.Options;
using SmallService.Shared;

namespace SmallService.Infrastructure.Abstractions.Messaging.RabbitMq;

public sealed class RabbitMq : IRabbitMq
{
    private IConnection _connection;
    private IModel _channel;
    private readonly RabbitMqOptions _options;

    public RabbitMq(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task Send<TModel>(string queueName, TModel model, Dictionary<string, string> messageAttributes = default) where TModel : class
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = _options.HostName };
            using (_connection = factory.CreateConnection())
            using (_channel = _connection.CreateModel())
            {
                _channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var json = JsonSerializer.Serialize(model);
                var body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);

            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(RabbitMq), methodName: nameof(Send), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> Receive<TModel>(string queueName) where TModel : class
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = _options.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            BasicGetResult result = _channel.BasicGet(queueName, true);
            if (result == null)
                return null;

            var data = Encoding.UTF8.GetString(result.Body.ToArray());
            var returnObject = JsonSerializer.Deserialize<TModel>(data);
            return await Task.FromResult<TModel>(returnObject);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(RabbitMq), methodName: nameof(Receive), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}