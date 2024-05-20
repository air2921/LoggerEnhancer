using Microsoft.Extensions.Logging;

namespace LoggerEnhancer.Abstractions
{
    public interface IContext
    {
        public string ContextId { get; set; }
        public bool IsDateLog { get; set; }
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
        public HashSet<LogLevel>? IgnoreLevels { get; set; }
        public HashSet<string>? KeyIgnore { get; set; }
    }
}
