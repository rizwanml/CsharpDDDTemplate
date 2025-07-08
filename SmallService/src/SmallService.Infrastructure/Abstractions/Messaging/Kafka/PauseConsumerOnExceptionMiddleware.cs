using System.Runtime.ExceptionServices;
using KafkaFlow;
using KafkaFlow.Consumers;
using SmallService.Shared;

namespace SmallService.Infrastructure.Abstractions.Messaging.Kafka;

public class PauseConsumerOnExceptionMiddleware : IMessageMiddleware
{
    private readonly IConsumerAccessor _consumerAccessor;
    private readonly ILogHandler _logHandler;

    public PauseConsumerOnExceptionMiddleware(IConsumerAccessor consumerAccessor, ILogHandler logHandler)
    {
            _consumerAccessor = consumerAccessor;
            _logHandler = logHandler;
        }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var errorDetail = $"Message: {context.Message}, " +
                                    $"Topic: {context.ConsumerContext.Topic}, " +
                                    $"MessageKey: {context.Message.Key}, " +
                                    $"ConsumerName: {context.ConsumerContext.ConsumerName}";

                ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PauseConsumerOnExceptionMiddleware), methodName: nameof(Invoke),
                                                        operationDetail: errorDetail);
                ExceptionDispatchInfo.Capture(ex).Throw();

                context.ConsumerContext.AutoMessageCompletion = false;

                var consumer = _consumerAccessor[context.ConsumerContext.ConsumerName];
                consumer.Pause(consumer.Assignment);

                _logHandler.Warning("Consumer stopped", context.ConsumerContext.ConsumerName);
            }
        }
}