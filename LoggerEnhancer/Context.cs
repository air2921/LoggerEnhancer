using LoggerEnhancer.Abstractions;

namespace LoggerEnhancer
{
    public class Context : IContext
    {
        public string ContextId { get; set; } = "None";
        public bool IsDateLog { get; set; }
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
        public IReadOnlySet<string>? KeyIgnore { get; set; }
    }
}
