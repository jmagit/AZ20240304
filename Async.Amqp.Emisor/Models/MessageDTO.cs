namespace Async.Amqp.Emisor.Models {
    public class MessageDTO {
        public string Msg { get; set; }
        public string Origen { get; set; }
        public DateTime Enviado { get; set; } = DateTime.Now;

        public MessageDTO() { }
        public MessageDTO(string msg, string origen) {
            Msg = msg;
            Origen = origen;
            Enviado = DateTime.Now;
        }
    }
}
