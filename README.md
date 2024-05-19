<div align="center">

# LoggerEnhancer

</div>

*LoggerEnhancer is an implementation standard `ILogger<T>` interface to enhance basic logging from `ASP.NET Core.`.*

*LoggerEnhancer allows you to add any additional information to each log.*

*The logger does not require changing the logging interface. It uses the standard `ILogger<T>` interface from Microsoft*

## Usage
```csharp
using LoggerEnhancer;

public void ConfigureServices(IServiceCollection services)
{
    // Register your other dependencies
    services.AddLogging();
    services.AddLoggerEnhancer(); // Add LoggerEnhancer service from LoggerEnhancer namespace
}
```

In this example, contextual information will be recorded from custom middleware

```csharp
using LoggerEnhancer.Abstractions;

public class LogMiddleware(RequestDelegate next, IContext context)
{
    public async Task Invoke(HttpContext httpContext)
    {
        var pairs = new Dictionary<string, string>();
        pairs.Add("Username", "air2921");
        pairs.Add("uID", 2921.ToString());

        // ContextId. default value is: "None"
        context.ContextId = Guid.NewGuid().ToString();

        // Contextual information
        context.Pairs = pairs;

        // Flag that determines whether it is necessary to register the log recording time
        context.IsDateLog = true;

        // Adding the names of the keys that must be ignored when writing the log
        // pairs.Add("Username", "air2921"); Will be ignored
        context.KeyIgnore = new HashSet<string> { "Username" };

        // Add logging levels that need to be ignored
        context.IgnoreLevels = new HashSet<LogLevel> { LogLevel.Information };

        await next(httpContext);
        return;
    }
}

public static class LogMiddlewareExtensions
{
    public static IApplicationBuilder UseLog(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogMiddleware>();
    }
}
```

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Add your anothers middlewares
    app.UseLog();
    // Add your anothers middlewares
}
```

Now you can use the standard `ILogger<T>` interface to write logs.

```csharp
public class LoggerController(ILogger<LoggerController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult TestLog()
    {
        logger.LogWarning("Here is your any log message"); // Using LogWarning because of LogInformation is ignored
        return StatusCode(200, new { message = "OK" });
    }
}
```

Example log entry:

```
warn: webapi.Controllers.LoggerController[0]
      <Context Id>a665fd12-d23d-4cbf-bb99-538f89506fd6</Context Id>
      <Logging Date>5/19/2024 4:42:43 AM</Logging Date>
      <uID>2921</uID>
      <Original log>Here is your any log message</Original log>
```

If you need to exclude contextual information from a specific log, you can use the `ILogger Enhancer<T>` interface to pass the parameter:

```csharp
public class LoggerController(ILoggerEnhancer<LoggerController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult TestLog()
    {
        logger.LogWarning("Here is your any log message", contextIgnore: true); // Using LogWarning because of LogInformation is ignored
        return StatusCode(200, new { message = "OK" });
    }
}
```

Example log entry if context ignored:

```
warn: webapi.Controllers.LoggerController[0]
      Here is your any log message
```
