using Async.Amqp.Emisor.Models;

namespace Async.Amqp.Receptor.Models {
    public class Respuesta {
        public MessageDTO Message { get; set; }
        public DateTime Recibido { get; init; } = DateTime.Now;
    }
}
