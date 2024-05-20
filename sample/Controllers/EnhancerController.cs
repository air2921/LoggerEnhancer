using LoggerEnhancer.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace sample.Controllers
{
    [Route("api/enhancer")]
    [ApiController]
    public class EnhancerController : ControllerBase
    {
        private readonly ILogger<EnhancerController> _logger;
        private readonly ILoggerEnhancer<EnhancerController> _loggerEnhancer;

        public EnhancerController(
            ILogger<EnhancerController> logger,
            ILoggerEnhancer<EnhancerController> loggerEnhancer)
        {
            _logger = logger;
            _loggerEnhancer = loggerEnhancer;
        }

        [HttpGet("iLogger")]
        public IActionResult UseILogger()
        {
            _logger.LogInformation($"This log from {nameof(UseILogger)} and it's {nameof(_logger)} dependency");
            return Ok();
        }

        [HttpGet("iLoggerEnhancer")]
        public IActionResult UseILoggerEnhancer()
        {
            _loggerEnhancer.LogInformation($"Context for this log will be ignored", contextIgnore: true);
            _loggerEnhancer.LogInformation($"This log from {nameof(UseILoggerEnhancer)} and it's {nameof(_loggerEnhancer)} dependency");
            return Ok();
        }
    }
}
