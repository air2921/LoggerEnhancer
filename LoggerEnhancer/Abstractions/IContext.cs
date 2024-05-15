namespace LoggerEnhancer.Abstractions
{
    public interface IContext
    {
        public string ContextId { get; set; }
        public Dictionary<string, string>? Pairs { get; set; }
    }
}
