using AsyncAmqpReceptor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AsyncAmqpReceptor.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RecibidosController : ControllerBase {
        [HttpGet]
        public IActionResult Index() {
            return Ok(Store.Peticiones);
        }
    }
}
