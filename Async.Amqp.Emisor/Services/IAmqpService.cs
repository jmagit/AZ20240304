namespace Async.Amqp.Emisor.Services {
    public interface IAmqpService {
        void Send(object message);
        void Send(string message);
    }
}