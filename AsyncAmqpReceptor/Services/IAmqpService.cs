namespace AsyncAmqpEmisor.Services {
    public interface IAmqpService {
        void Send(object message);
        void Send(string message);
    }
}