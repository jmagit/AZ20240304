using AsyncAmqpEmisor.Models;

namespace AsyncAmqpReceptor.Models {
    public class Store {
        public static List<Peticion> Peticiones { get; } = new List<Peticion>();

        public static void Add(MessageDTO message) {
            Peticiones.Add(new Peticion { Message = message });
        }


    }
}
