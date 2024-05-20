using LoggerEnhancer;
using LoggerEnhancer.Abstractions;
using LoggerEnhancer.Attributes;
using Microsoft.Extensions.Logging;
using Moq;

namespace tests
{
    public class EnhancerTest
    {
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IContext> _contextMock;

        public EnhancerTest()
        {
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();
            _contextMock = new Mock<IContext>();

            _loggerFactoryMock.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
        }

        [Fact]
        public void Log_FullLog_NoIgnore()
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(true);
            _contextMock.Setup(x => x.IgnoreLevels).Returns((HashSet<LogLevel>)null);
            _contextMock.Setup(x => x.KeyIgnore).Returns((HashSet<string>)null);

            var enhancer = new Enhancer<object>(_loggerFactoryMock.Object, _contextMock.Object);
            enhancer.LogInformation("Log for test");

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsRequiredSubstrings(v, "Context")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Fact]
        public void Log_DontLogDate()
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(false);
            _contextMock.Setup(x => x.IgnoreLevels).Returns((HashSet<LogLevel>)null);
            _contextMock.Setup(x => x.KeyIgnore).Returns((HashSet<string>)null);

            var enhancer = new Enhancer<object>(_loggerFactoryMock.Object, _contextMock.Object);
            enhancer.LogInformation("Log for test");

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsRequiredDoesNotContainsDateSubstrings(v, "Context")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Fact]
        public void Log_KeyIgnore()
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(true);
            _contextMock.Setup(x => x.IgnoreLevels).Returns((HashSet<LogLevel>)null);
            _contextMock.Setup(x => x.KeyIgnore).Returns(new HashSet<string>() { "TestKey" });

            var enhancer = new Enhancer<object>(_loggerFactoryMock.Object, _contextMock.Object);
            enhancer.LogInformation("Log for test");

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => ContainsRequiredAndDoesNotContainsIgnoredSubstrings(v)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Theory]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        public void Log_LogLevelIgnore(LogLevel level)
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(true);
            _contextMock.Setup(x => x.IgnoreLevels).Returns(new HashSet<LogLevel> { level });
            _contextMock.Setup(x => x.KeyIgnore).Returns((HashSet<string>)(null));

            var enhancer = new Enhancer<object>(_loggerFactoryMock.Object, _contextMock.Object);

            switch (level)
            {
                case LogLevel.Debug:
                    enhancer.LogDebug("Log for test");
                    break;
                case LogLevel.Information:
                    enhancer.LogInformation("Log for test");
                    break;
                case LogLevel.Warning:
                    enhancer.LogWarning("Log for test");
                    break;
                case LogLevel.Error:
                    enhancer.LogError("Log for test");
                    break;
                case LogLevel.Critical:
                    enhancer.LogCritical("Log for test");
                    break;
            }

            _loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Context")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never());
        }

        [Fact]
        public void Log_ContextIgnore_Attribute()
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(true);
            _contextMock.Setup(x => x.IgnoreLevels).Returns((HashSet<LogLevel>)(null));
            _contextMock.Setup(x => x.KeyIgnore).Returns((HashSet<string>)(null));

            var enhancer = new Enhancer<ClassIgnore>(_loggerFactoryMock.Object, _contextMock.Object);

            enhancer.LogInformation("Log for test");

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v == "Log for test"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        [Fact]
        public void Log_ContextIgnore_Param()
        {
            _contextMock.Setup(x => x.ContextId).Returns("Context");
            _contextMock.Setup(x => x.Pairs).Returns(new Dictionary<string, string>() { { "TestKey", "2921" } });
            _contextMock.Setup(x => x.IsDateLog).Returns(true);
            _contextMock.Setup(x => x.IgnoreLevels).Returns((HashSet<LogLevel>)(null));
            _contextMock.Setup(x => x.KeyIgnore).Returns((HashSet<string>)(null));

            var enhancer = new Enhancer<object>(_loggerFactoryMock.Object, _contextMock.Object);

            enhancer.LogInformation("Log for test", contextIgnore: true);

            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v == "Log for test"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }

        private bool ContainsRequiredAndDoesNotContainsIgnoredSubstrings(object state)
        {
            var logMessage = state.ToString();
            return !logMessage.Contains($"<TestKey>{"2921"}</TestKey>") &&
                logMessage.Contains($"<Context Id>{"Context"}</Context Id>") &&
                logMessage.Contains($"<Logging Date>") &&
                logMessage.Contains($"<Original log>{"Log for test"}</Original log>");
        }

        private bool ContainsRequiredSubstrings(object state, string name)
        {
            var logMessage = state.ToString();
            return logMessage.Contains($"<TestKey>{"2921"}</TestKey>") &&
                logMessage.Contains($"<Context Id>{name}</Context Id>") &&
                logMessage.Contains($"<Logging Date>") &&
                logMessage.Contains($"<Original log>{"Log for test"}</Original log>");
        }

        private bool ContainsRequiredDoesNotContainsDateSubstrings(object state, string name)
        {
            var logMessage = state.ToString();
            return logMessage.Contains($"<TestKey>{"2921"}</TestKey>") &&
                logMessage.Contains($"<Context Id>{name}</Context Id>") &&
                !logMessage.Contains($"<Logging Date>") &&
                logMessage.Contains($"<Original log>{"Log for test"}</Original log>");
        }
    }

    [ContextIgnore]
    public class ClassIgnore
    {

    }
}
