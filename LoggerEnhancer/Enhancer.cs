using LoggerEnhancer.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LoggerEnhancer
{
    public class Enhancer<T>(ILoggerFactory loggerFactory, IContext context) : ILogger<T>
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(T).FullName ?? typeof(T).Name) ??
            throw new ArgumentNullException(nameof(loggerFactory), "Logger cannot be null");

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state)!;

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            var stateBuilder = new StringBuilder();
            stateBuilder.Append($"Context Id: {context.ContextId}");

            if (context.Pairs is not null)
                foreach (var pair in context.Pairs)
                    stateBuilder.Append($"\n{pair.Key}: {pair.Value}");

            stateBuilder.Append($"\n{state}");

            _logger.Log(logLevel, eventId, stateBuilder.ToString(), exception, formatter);
        }
    }
}
