using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Async.Amqp.Emisor.Services {
    public class RabbitMQClientService : IAmqpService, IDisposable {
        private readonly ConnectionFactory factory;
        private readonly IConnection connection;
        private IModel channel;

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
            channel.QueueDeclare(queue: "demo.peticiones",
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

        public void Send(object message, string exchange = "", string routingKey = "",
            CancellationToken cancellationToken = default, EventHandler<BasicDeliverEventArgs>? callback = null) {
            if(message == null) {
                throw new ArgumentNullException("message");
            }
            Send(JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                exchange, routingKey, cancellationToken, callback);
        }
        public void Send(string message, string exchange = "", string routingKey = "",
            CancellationToken cancellationToken = default, EventHandler<BasicDeliverEventArgs>? callback = null) {
            if(channel.IsClosed) 
                channel = connection.CreateModel();
            IBasicProperties props = channel.CreateBasicProperties();
            if(callback != null) {
                var replyQueueName = "demo.peticiones";
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += callback;
                channel.BasicConsume(consumer: consumer,
                             queue: replyQueueName,
                             autoAck: true);
                props.CorrelationId = Guid.NewGuid().ToString();
                props.ReplyTo = replyQueueName;
            }

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: props,
                                 body: body);
        }
    }
}
