using Async.Amqp.Emisor.Models;

namespace Async.Amqp.Receptor.Models {
    public class Store {
        public static List<Respuesta> Respuestas { get; } = new List<Respuesta>();

        public static void Add(MessageDTO message) {
            Respuestas.Add(new Respuesta { Message = message });
        }

    }
}
