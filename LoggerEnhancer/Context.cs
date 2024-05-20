using LoggerEnhancer.Abstractions;
using Microsoft.Extensions.Logging;

namespace LoggerEnhancer
{
    public class Context : IContext
    {
        public string ContextId { get; set; } = "None";
        public bool IsDateLog { get; set; }
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
        public HashSet<LogLevel>? IgnoreLevels { get; set; }
        public HashSet<string>? KeyIgnore { get; set; }
    }
}
