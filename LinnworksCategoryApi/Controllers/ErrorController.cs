using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LinnworksCategoryApi.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/Error-Develop")]
        public IActionResult Index()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            _logger.LogError(context.Error.InnerException, context.Error.Message);

            return new JsonResult(context.Error.Message);
        }

        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}