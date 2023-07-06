using ApiGateway.Models;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ApiGateway.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        //private readonly string _connectionString;
        //private readonly string _queueName;

        //public RabbitMqService(string connectionString, string queueName)
        //{
        //    _connectionString = connectionString;
        //    _queueName = queueName;
        //}

        public void SendMessage(AmqpMessage message)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                var body = Encoding.UTF8.GetBytes(message.Message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: message.Exchange, routingKey: message.RoutingKey, basicProperties: properties, body: body);
            }
        }

        public async Task<AmqpMessage> GetMessage(string queueName)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                var tcs = new TaskCompletionSource<AmqpMessage>();
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var amqpMessage = new AmqpMessage { Message = message };
                    tcs.SetResult(amqpMessage);
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                return await tcs.Task;
                // BasicGetResult result = channel.BasicGet(queueName, autoAck: true);

                //if (result != null)
                //{
                //    var body = result.Body.ToArray();
                //    var message = Encoding.UTF8.GetString(body);

                //    return new AmqpMessage { Message = message };
                //}

                //return null;
            }
        }
    }
}
