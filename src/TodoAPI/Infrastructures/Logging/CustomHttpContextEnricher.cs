namespace TodoAPI.Infrastructures.Logging;

public class CustomHttpContextEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomHttpContextEnricher()
        : this(new HttpContextAccessor()) { }

    public CustomHttpContextEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return;
        string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!String.IsNullOrEmpty(userId))
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("X-UserId", userId));
        }
    }
}
