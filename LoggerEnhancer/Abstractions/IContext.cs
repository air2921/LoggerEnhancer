namespace LoggerEnhancer.Abstractions
{
    public interface IContext
    {
        public string ContextId { get; set; }
        public IReadOnlyDictionary<string, string>? Pairs { get; set; }
    }
}
