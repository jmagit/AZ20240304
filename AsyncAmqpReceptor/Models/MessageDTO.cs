using System.Text.Json.Serialization;

namespace AsyncAmqpEmisor.Models {
    public class MessageDTO {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }
        [JsonPropertyName("origen")]
        public string Origen { get; set; }
        [JsonPropertyName("enviado")]
        public DateTime Enviado { get; set; } = DateTime.Now;

        public MessageDTO() { }
        public MessageDTO(string msg, string origen) {
            Msg = msg;
            Origen = origen;
            Enviado = DateTime.Now;
        }
    }
}
