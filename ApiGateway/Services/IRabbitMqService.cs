using ApiGateway.Models;

namespace ApiGateway.Services
{
    public interface IRabbitMqService
    {
        void SendMessage(AmqpMessage message);
        Task<AmqpMessage> GetMessage(string queueName);
    }
}
