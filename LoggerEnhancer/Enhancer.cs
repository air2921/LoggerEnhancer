using LoggerEnhancer.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LoggerEnhancer
{
    public class Enhancer<T>(ILoggerFactory loggerFactory, IContext context) : ILogger<T>, ILoggerEnhancer<T>
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(T).FullName ?? typeof(T).Name) ??
            throw new ArgumentNullException(nameof(loggerFactory), "Logger cannot be null");

        protected void LogInternal<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception, string> formatter, bool contextIgnore)
        {
            if (context.IgnoreLevels is not null && context.IgnoreLevels.Contains(logLevel))
                return;

            if (contextIgnore)
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
                return;
            }

            var log = DefaultContextLog(state, exception, formatter);
            _logger.Log(logLevel, eventId, log, exception, formatter);
        }

        private string DefaultContextLog<TState>(TState state, Exception? exception,
            Func<TState, Exception, string> formatter)
        {
            var stateBuilder = new StringBuilder(256);
            stateBuilder.AppendLine($"<Context Id>{context.ContextId}</Context Id>");

            if (context.IsDateLog)
                stateBuilder.AppendLine($"<Logging Date>{DateTime.UtcNow}</Logging Date>");

            if (context.Pairs is not null && context.Pairs.Count > 0)
            {
                foreach (var pair in context.Pairs)
                {
                    if (context.KeyIgnore is null || !context.KeyIgnore.Contains(pair.Key))
                    {
                        stateBuilder.AppendLine($"<{pair.Key}>{pair.Value}</{pair.Key}>");
                    }
                }
            }

            stateBuilder.AppendLine($"<Original log>{state}</Original log>");
            if (exception is not null && state is not null)
                stateBuilder.AppendLine(formatter is not null ? formatter(state, exception) : state.ToString());

            return stateBuilder.ToString();
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state) ??
            throw new ArgumentNullException(nameof(state), "Logger state cannot be null");

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception, string> formatter)
        {
            LogInternal(logLevel, eventId, state, exception, formatter, false);
        }

        public void LogDebug<TState>(TState state, bool contextIgnore = false)
        {
            LogInternal(LogLevel.Debug, new EventId(), state, null, (s, e) => s.ToString(), contextIgnore);
        }

        public void LogInformation<TState>(TState state, bool contextIgnore = false)
        {
            LogInternal(LogLevel.Information, new EventId(), state, null, (s, e) => s.ToString(), contextIgnore);
        }

        public void LogWarning<TState>(TState state, bool contextIgnore = false)
        {
            LogInternal(LogLevel.Warning, new EventId(), state, null, (s, e) => s.ToString(), contextIgnore);
        }

        public void LogError<TState>(TState state, Exception? exception = null, bool contextIgnore = false)
        {
            LogInternal(LogLevel.Error, new EventId(), state, exception, (s, e) => s.ToString(), contextIgnore);
        }

        public void LogCritical<TState>(TState state, bool contextIgnore = false)
        {
            LogInternal(LogLevel.Critical, new EventId(), state, null, (s, e) => s.ToString(), contextIgnore);
        }
    }
}
