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
            stateBuilder.Append($"Context Id > {context.ContextId}");

            if (context.Pairs is not null && context.Pairs.Count > 0)
                foreach (var pair in context.Pairs)
                    stateBuilder.AppendLine($"{pair.Key} > {pair.Value}");

            stateBuilder.AppendLine($"Original log > {state}");

            if (exception is not null && state is not null)
                stateBuilder.AppendLine(formatter is not null ? formatter(state, exception) : state.ToString());

            _logger.Log(logLevel, eventId, stateBuilder.ToString(), exception, formatter);
        }
    }
}
