using Microsoft.Extensions.Logging;

namespace LoggerEnhancer.Abstractions
{
    public interface IContext
    {
        public string ContextId { get; set; }
        public bool IsDateLog { get; set; }
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
        public IReadOnlySet<LogLevel>? IgnoreLevels { get; set; }
        public IReadOnlySet<string>? KeyIgnore { get; set; }
    }
}
