using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Async.Amqp.Emisor.Services {
    public class RabbitMQClientService : IAmqpService, IDisposable {
        private readonly ConnectionFactory factory;
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabbitMQClientService() {
            factory = new ConnectionFactory {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "curso",
                ClientProvidedName = "Async.Amqp.Emisor"
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "demo.saludos",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            channel.ExchangeDeclare("demo.rpc", ExchangeType.Direct, true);

        }

        public void Dispose() {
            channel.Dispose();
            connection.Dispose();
        }

        public void Send(object message, string exchange = "", string routingKey = "", CancellationToken cancellationToken = default) {
            if(message == null) {
                throw new ArgumentNullException("message");
            }
            Send(JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
        public void Send(string message, string exchange = "", string routingKey = "", CancellationToken cancellationToken = default) {

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "demo.saludos",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
