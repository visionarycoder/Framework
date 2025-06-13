using Azure;
using Azure.Storage.Queues.Models;

namespace v8.Ifx.Services.Cloud.Azure.Storage;

public interface IQueueConnector
{
    QueueMessage? ReceiveMessage(string queueName);
    Task<QueueMessage?> ReceiveMessageAsync(string queueName);

    Response<SendReceipt> SendMessage(string queueName, string message);
    Task<Response<SendReceipt>> SendMessageAsync(string queueName, string message);
    
}