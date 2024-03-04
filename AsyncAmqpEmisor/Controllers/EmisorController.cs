using AsyncAmqpEmisor.Models;
using AsyncAmqpEmisor.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AsyncAmqpEmisor.Controllers {
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
            srv.Send(new MessageDTO($"Hola Don {nombre}", Request.Host.Port.ToString()));
            return this.Accepted();
        }
        [HttpGet("multiple")]
        public IActionResult multiple([FromQuery] int cantidad = 10) {
            for(int i = 1; i <= cantidad; i++) {
                _logger.Log(LogLevel.Information, $"Send: Envio nº {i}");
                srv.Send(new MessageDTO($"Envio nº {i}", Request.Host.Port.ToString()));
            }
            return this.Accepted();
        }
    }
}
