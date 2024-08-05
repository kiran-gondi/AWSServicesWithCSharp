using Amazon.SimpleNotificationService.Model;

namespace Customers.Api.Messaging;

public interface ISnsMessenger
{
    // Task<SendMessageResponse> SendMessageAsync<T>(T message);
    Task<PublishResponse> PublishMessageAsync<T>(T message);
}