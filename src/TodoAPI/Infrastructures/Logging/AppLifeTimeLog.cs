namespace TodoAPI.Infrastructures.Logging;

public static class AppLifeTimeLog
{
    public static void LogAppLifeTime(this WebApplication app)
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            app.Logger.LogInformation(
                $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} ApplicationStopping"
            );
        });
        app.Lifetime.ApplicationStopped.Register(() =>
        {
            app.Logger.LogInformation($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} ApplicationStopped");
        });
    }
}
