using LoggerEnhancer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace sample.Controllers
{
    [Route("api/enhancer")]
    [ApiController]
    [ContextIgnore]
    public class EnhancerControllerWithAttribute : ControllerBase
    {
        private readonly ILogger<EnhancerController> _logger;

        public EnhancerControllerWithAttribute(ILogger<EnhancerController> logger)
        {
            _logger = logger;
        }

        [HttpGet("iLoggerIgnore")]
        public IActionResult UseILoggerWithIgnore()
        {
            _logger.LogInformation("Context for this log will be ignored");
            return Ok();
        }
    }
}
