using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingResourceFilter : IResourceFilter
{
    private readonly ILogger<LoggingResourceFilter> _logger;

    public LoggingResourceFilter(ILogger<LoggingResourceFilter> logger)
    {
        _logger = logger;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        _logger.LogInformation(">>> Resource Filter BEFORE action: {Path}", context.HttpContext.Request.Path);
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        _logger.LogInformation("<<< Resource Filter AFTER action: {Path}", context.HttpContext.Request.Path);
    }
}