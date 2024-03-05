using Async.Amqp.Receptor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Async.Amqp.Receptor.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RecibidosController : ControllerBase {
        [HttpGet]
        public IActionResult Index() {
            return Ok(Store.Peticiones.OrderByDescending(p => p.Recibido));
        }
    }
}
