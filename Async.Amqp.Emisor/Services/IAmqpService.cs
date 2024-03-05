using RabbitMQ.Client.Events;

namespace Async.Amqp.Emisor.Services {
    public interface IAmqpService {
        void Send(object message, string exchange = "", string routingKey = "", 
            CancellationToken cancellationToken = default, EventHandler<BasicDeliverEventArgs>? callback = null);
        void Send(string message, string exchange = "", string routingKey = "", 
            CancellationToken cancellationToken = default, EventHandler<BasicDeliverEventArgs>? callback = null);
    }
}