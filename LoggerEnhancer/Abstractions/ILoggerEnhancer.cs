using Microsoft.Extensions.Logging;

namespace LoggerEnhancer.Abstractions
{
    public interface ILoggerEnhancer<T> : ILogger<T>
    {
        void LogDebug<TState>(TState state, bool contextIgnore = false);
        void LogInformation<TState>(TState state, bool contextIgnore = false);
        void LogWarning<TState>(TState state, bool contextIgnore = false);
        void LogError<TState>(TState state, Exception? exception = null, bool contextIgnore = false);
        void LogCritical<TState>(TState state, bool contextIgnore = false);
    }
}
