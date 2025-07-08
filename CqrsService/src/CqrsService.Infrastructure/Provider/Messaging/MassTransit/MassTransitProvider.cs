using System.Runtime.ExceptionServices;
using CqrsService.Shared;
using MassTransit;

namespace CqrsService.Infrastructure.Provider.Messaging.MassTransit;

public sealed class MassTransitProvider : IMassTransitProvider
{
    private readonly IBusControl _busControl;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMessageScheduler _messageScheduler;

    public MassTransitProvider(IBusControl busControl, IPublishEndpoint publishEndpoint, IMessageScheduler messageScheduler)
    {
        _busControl = busControl;
        _publishEndpoint = publishEndpoint;
        _messageScheduler = messageScheduler;
    }

    /// <summary>
    /// Sends a message that is consumed by the first available consumer listening on the given queue name only
    /// </summary>
    /// <typeparam name="TModel">The message object type</typeparam>
    /// <param name="queueName">The queue name to send the message on</param>
    /// <param name="model">The message object</param>
    /// <param name="delay">An optional delay before adding the message to the queue</param>
    /// <returns></returns>
    public async Task Send<TModel>(string queueName, TModel model, TimeSpan? delay = null) where TModel : class
    {
        try
        {
            var sendEndpoint = new Uri($"queue:{queueName}");
            var sendEndpointProvider = await _busControl.GetSendEndpoint(sendEndpoint);

            if (delay.HasValue)
            {
                var timeToSend = DateTime.UtcNow;
                timeToSend.Add(delay.Value);

                await _messageScheduler.ScheduleSend(sendEndpoint, timeToSend, model);
            }
            else
            {
                await sendEndpointProvider.Send(model);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MassTransitProvider), methodName: nameof(Send), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    /// <summary>
    /// Publishes a message that is consumed by all available consumers that are registered to the given message type
    /// </summary>
    /// <typeparam name="TModel">The message object type</typeparam>
    /// <param name="model">The message object</param>
    /// <param name="delay">An optional delay before adding the message to the queue</param>
    /// <returns></returns>
    public async Task Publish<TModel>(TModel model, TimeSpan? delay = null) where TModel : class
    {
        try
        {
            if (delay.HasValue)
            {
                DateTime timeToSend = DateTime.UtcNow;
                timeToSend.Add(delay.Value);

                await _messageScheduler.SchedulePublish(timeToSend, model);
            }
            else
            {
                await _publishEndpoint.Publish(model);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MassTransitProvider), methodName: nameof(Publish));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}