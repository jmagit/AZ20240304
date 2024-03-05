using Async.Amqp.Emisor.Models;
using Async.Amqp.Emisor.Services;
using Async.Amqp.Receptor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace Async.Amqp.Emisor.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmisorController : ControllerBase {
        private readonly ILogger<EmisorController> _logger;
        private readonly IAmqpService srv;
        public EmisorController(ILogger<EmisorController> logger, IAmqpService srv) {
            _logger = logger;
            this.srv = srv;
        }

        [HttpGet("saludo")]
        public IActionResult Saluda([FromQuery] string nombre = "mundo") {
            _logger.Log(LogLevel.Information, $"Send: Hola don {nombre}");
#pragma warning disable CS8604 // Posible argumento de referencia nulo
            srv.Send(new MessageDTO($"Hola Don {nombre}", Request.Host.Port.ToString()), routingKey: "demo.saludos");
#pragma warning restore CS8604 // Posible argumento de referencia nulo
            return Accepted();
        }

        [HttpGet("multiple")]
        public IActionResult Multiple([FromQuery] int cantidad = 10) {
            for(int i = 1; i <= cantidad; i++) {
                _logger.Log(LogLevel.Information, $"Send: Envio nº {i}");
#pragma warning disable CS8604 // Posible argumento de referencia nulo
                srv.Send(new MessageDTO($"Envio nº {i}", Request.Host.Port.ToString()), routingKey: "demo.saludos");
#pragma warning restore CS8604 // Posible argumento de referencia nulo
            }
            return Accepted();
        }

        [HttpGet("rpc")]
        public IActionResult Rpc([FromQuery] string nombre = "mundo") {
            _logger.Log(LogLevel.Information, $"Send: Hola don {nombre}");
            srv.Send(
                new MessageDTO($"Hola Don {nombre}", Request.Host.Port.ToString()),
                routingKey: "demo.peticiones",
                //exchange: "demo.rpc",
                callback: (model, ev) => {
                    var canal = (model as EventingBasicConsumer).Model;
                    var body = Encoding.UTF8.GetString(ev.Body.ToArray());
                    var respuesta = JsonSerializer.Deserialize<MessageDTO>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if(respuesta != null) Store.Add(respuesta);
                    //canal.BasicAck(ev.DeliveryTag, false);
                }
                );
            return Accepted();
        }
        [HttpGet("respuestas")]
        public IActionResult Respuestas() {
            return Ok(Store.Respuestas.OrderByDescending(p => p.Recibido));
        }
    }
}
