using AsyncAmqpEmisor.Models;

namespace AsyncAmqpReceptor.Models {
    public class Peticion {
        public MessageDTO Message { get; set; }
        public DateTime Recibido { get; init; } = DateTime.Now;
    }
}
