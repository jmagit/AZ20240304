using Async.Amqp.Emisor.Models;

namespace Async.Amqp.Receptor.Models {
    public class Store {
        public static List<Peticion> Peticiones { get; } = new List<Peticion>();

        public static void Add(MessageDTO message) {
            Peticiones.Add(new Peticion { Message = message });
        }


    }
}
