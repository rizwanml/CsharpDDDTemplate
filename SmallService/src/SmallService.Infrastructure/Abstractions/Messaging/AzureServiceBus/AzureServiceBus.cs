using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus.Models;
using SmallService.Shared;

namespace SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus;

public sealed class AzureServiceBus : IAzureServiceBus
{
    private readonly ServiceBusClientOptions _clientOptions;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public AzureServiceBus(ServiceBusClientOptions clientOptions)
    {
        _clientOptions = clientOptions;
    }

    public async Task Send<TModel>(string connectionString, string queueName, TModel message, AzureServiceBusMessageOptions? options = null) where TModel : class
    {
        try
        {
            var senderOptions = new ServiceBusSenderOptions()
            {
                Identifier = options?.Identifier
            };

            var messageBody = JsonSerializer.Serialize(message, _serializerOptions);
                
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                ContentType = options?.ContentType,
                CorrelationId = options?.CorrelationId,
                MessageId = options?.MessageId,
                PartitionKey = options?.PartitionKey,
                ReplyTo = options?.ReplyTo,
                ReplyToSessionId = options?.ReplyToSessionId,
                Subject = options?.Subject,
                TransactionPartitionKey = options?.TransactionPartitionKey
            };
                
            if (options?.ScheduledEnqueueTime.HasValue == true)
            {
                serviceBusMessage.ScheduledEnqueueTime = options.ScheduledEnqueueTime.Value;
            }

            if (options?.TimeToLiveSeconds.HasValue == true)
            {
                serviceBusMessage.TimeToLive = TimeSpan.FromSeconds(options.TimeToLiveSeconds.Value);;
            }
                
            await using var client = new ServiceBusClient(connectionString, _clientOptions);
            await using var sender = client.CreateSender(queueName, senderOptions);
                
            await sender.SendMessageAsync(serviceBusMessage);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(AzureServiceBus), methodName: nameof(Send), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IReadOnlyList<AzureServiceBusReceiveResponse<TModel>>> Receive<TModel>(string connectionString, string queueName, string? identifier = null, int? prefetchCount = null, int maxNumberOfMessages = 10, int waitTimeSeconds = 20) where TModel : class
    {
        try
        {
            var receiverOptions = new ServiceBusReceiverOptions()
            {
                Identifier = identifier,
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };
                
            if (prefetchCount.HasValue)
            {
                receiverOptions.PrefetchCount = prefetchCount.Value;
            }
                
            await using var client = new ServiceBusClient(connectionString, _clientOptions);
            await using var receiver = client.CreateReceiver(queueName, receiverOptions);
                
            var receiveMessagesResponse = await receiver.ReceiveMessagesAsync(maxNumberOfMessages, TimeSpan.FromSeconds(waitTimeSeconds));
                
            IReadOnlyList<AzureServiceBusReceiveResponse<TModel>> response = receiveMessagesResponse
                .Select(message => new { Message = message, Body = JsonSerializer.Deserialize<TModel>(message.Body.ToString(), _serializerOptions) })
                .Where(x => x.Body != null)
                .Select(x => new AzureServiceBusReceiveResponse<TModel> 
                { 
                    Message = x.Message, 
                    Body = x.Body 
                })
                .ToList();
                
            return response;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(AzureServiceBus), methodName: nameof(Receive), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
        
    public async Task<bool> Delete(string connectionString, string queueName, long messageId)
    {
        try
        {
            await using var client = new ServiceBusClient(connectionString, _clientOptions);
            await using var receiver = client.CreateReceiver(queueName);

            var message = await receiver.PeekMessageAsync(messageId);
                
            if (message == null)
            {
                return false;
            }

            await receiver.CompleteMessageAsync(message);
                
            return true;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(AzureServiceBus), methodName: nameof(Delete), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task BatchDelete(string connectionString, string queueName, IReadOnlyList<AzureServiceBusBatchDeleteRequest> azureServiceBusBatchDeleteRequest)
    {
        try
        {
            await using var client = new ServiceBusClient(connectionString, _clientOptions);
            await using var receiver = client.CreateReceiver(queueName);
                
            foreach (var deleteMessage in azureServiceBusBatchDeleteRequest)
            {
                var message = await receiver.PeekMessageAsync(deleteMessage.MessageId);
                    
                if (message != null)
                {
                    await receiver.CompleteMessageAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(AzureServiceBus), methodName: nameof(BatchDelete), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}