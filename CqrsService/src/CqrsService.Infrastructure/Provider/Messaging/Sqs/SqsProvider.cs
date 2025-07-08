using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using CqrsService.Infrastructure.Provider.Messaging.Sqs.Models;
using CqrsService.Shared;

namespace CqrsService.Infrastructure.Provider.Messaging.Sqs;

public sealed class SqsProvider : ISqsProvider
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public SqsProvider(IAmazonSQS amazonSqs)
    {
        _amazonSqs = amazonSqs;
    }
    
     public async Task Send<TModel>(string queueName, TModel message, int? delaySeconds = null, string? messageDeduplicationId = null, string? messageGroupId = null, Dictionary<string, string>? messageAttributes = null) where TModel : class
    {
        try
        {
            var getQueueUrlResponse = await _amazonSqs.GetQueueUrlAsync(queueName);
            var queueUrl = getQueueUrlResponse.QueueUrl;

            var messageBody = JsonSerializer.Serialize(message, _serializerOptions);
                
            Dictionary<string, MessageAttributeValue>? messageAttributeValues = null;

            if (messageAttributes is not null)
            {
                messageAttributeValues = messageAttributes.ToDictionary(
                    item => item.Key,
                    item => new MessageAttributeValue 
                    { 
                        DataType = "String", 
                        StringValue = item.Value 
                    }
                );
            }
                
            var sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody,
                MessageDeduplicationId = messageDeduplicationId,
                MessageGroupId = messageGroupId,
                MessageAttributes = messageAttributeValues
            };
                
            if (delaySeconds.HasValue)
            {
                sendMessageRequest.DelaySeconds = delaySeconds.Value;
            }
                
            await _amazonSqs.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(SqsProvider), methodName: nameof(Send), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IReadOnlyList<SqsReceiveResponse<TModel>>> Receive<TModel>(string queueName, int maxNumberOfMessages = 10, int waitTimeSeconds = 20, int visibilityTimeout = 30) where TModel : class
    {
        try
        {
            var getQueueUrlResponse = await _amazonSqs.GetQueueUrlAsync(queueName);
            var queueUrl = getQueueUrlResponse.QueueUrl;

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = maxNumberOfMessages,
                WaitTimeSeconds = waitTimeSeconds,
                VisibilityTimeout = visibilityTimeout
            };

            var receiveMessageResponse = await _amazonSqs.ReceiveMessageAsync(receiveMessageRequest);
                
            IReadOnlyList<SqsReceiveResponse<TModel>> response = receiveMessageResponse.Messages
                .Select(message => new { Message = message, Body = JsonSerializer.Deserialize<TModel>(message.Body, _serializerOptions) })
                .Where(x => x.Body != null)
                .Select(x => new SqsReceiveResponse<TModel>
                {
                    Message = x.Message,
                    Body = x.Body
                })
                .ToList();
                
            return response;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(SqsProvider), methodName: nameof(Receive), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<bool> Delete(string queueName, string messageId)
    {
        try
        {
            var getQueueUrlResponse = await _amazonSqs.GetQueueUrlAsync(queueName);
            var queueUrl = getQueueUrlResponse.QueueUrl;
                
            await _amazonSqs.DeleteMessageAsync(queueUrl, messageId);
                
            return true;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(SqsProvider), methodName: nameof(Delete), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task BatchDelete(string queueName, IReadOnlyList<SqsBatchDeleteRequest> sqsBatchDeleteRequest)
    {
        try
        {
            var getQueueUrlResponse = await _amazonSqs.GetQueueUrlAsync(queueName);
            var queueUrl = getQueueUrlResponse.QueueUrl;
                
            if (sqsBatchDeleteRequest.Count > 0)
            {
                var entries = sqsBatchDeleteRequest.Select(result => new DeleteMessageBatchRequestEntry
                {
                    Id = result.MessageId,
                    ReceiptHandle = result.ReceiptHandle
                }).ToList();

                var request = new DeleteMessageBatchRequest
                {
                    QueueUrl = queueUrl,
                    Entries = entries
                };

                await _amazonSqs.DeleteMessageBatchAsync(request);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(SqsProvider), methodName: nameof(BatchDelete), operationDetail: queueName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}