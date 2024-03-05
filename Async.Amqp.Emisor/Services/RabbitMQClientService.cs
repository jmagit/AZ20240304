using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Async.Amqp.Emisor.Services {
    public class RabbitMQClientService : IAmqpService {

        public void Send(object message) {
            if(message == null) {
                throw new ArgumentNullException("message");
            }
            Send(JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
        public void Send(string message) {
            var factory = new ConnectionFactory {
                HostName = "localhost", Port = 5672,
                UserName = "admin", Password = "curso"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "demo.saludos",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "demo.saludos",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
