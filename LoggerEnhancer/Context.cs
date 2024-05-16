using LoggerEnhancer.Abstractions;

namespace LoggerEnhancer
{
    public class Context : IContext
    {
        public string ContextId { get; set; } = "None";
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
    }
}
