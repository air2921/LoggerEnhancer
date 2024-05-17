using LoggerEnhancer.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LoggerEnhancer
{
    public class Enhancer<T>(ILoggerFactory loggerFactory, IContext context) : ILogger<T>
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(T).FullName ?? typeof(T).Name) ??
            throw new ArgumentNullException(nameof(loggerFactory), "Logger cannot be null");

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state) ??
            throw new ArgumentNullException(nameof(state), "Logger state cannot be null");

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception, string> formatter)
        {
            var stateBuilder = new StringBuilder(256);
            stateBuilder.AppendLine($"Context Id -> {context.ContextId}");

            if (context.IsDateLog)
                stateBuilder.AppendLine($"Logging time -> {DateTime.UtcNow}");

            if (context.Pairs is not null && context.Pairs.Count > 0)
            {
                if (context.KeyIgnore is not null && context.KeyIgnore.Count > 0 && context.Pairs.Keys.Any(key => context.KeyIgnore.Contains(key)))
                {
                    foreach (var pair in context.Pairs)
                        if (!context.KeyIgnore.Contains(pair.Key))
                            stateBuilder.AppendLine($"{pair.Key} -> {pair.Value}");
                }
                else
                {
                    foreach (var pair in context.Pairs)
                        stateBuilder.AppendLine($"{pair.Key} -> {pair.Value}");
                }
            }

            stateBuilder.AppendLine($"<Original log> {state}");

            if (exception is not null && state is not null)
                stateBuilder.AppendLine(formatter is not null ? formatter(state, exception) : state.ToString());

            _logger.Log(logLevel, eventId, stateBuilder.ToString(), exception, formatter);
        }
    }
}
