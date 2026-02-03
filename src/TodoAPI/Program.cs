var builder = WebApplication.CreateBuilder(args);

// Tips: 我使用 Cursor 開啟中斷點偵錯時，無法正確取得 ContentRootPath，所以需要手動設定
// var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "";
// var contentRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", ".."));
// var builder = WebApplication.CreateBuilder(
//     new WebApplicationOptions { ContentRootPath = contentRoot }
// );

SerilogConfig.AddSerilLog(builder.Configuration, builder.Environment);

try
{
    builder.Services.AddDI();
    builder.Services.AddOpenAPI();
    builder.Services.AddDapper();
    builder.Services.AddRepositories();
    builder.Services.AddEFCore(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddJWT(builder.Configuration);
    builder.Services.AddSecurity();
    builder.Services.AddExceptionHandlerConfig();
    builder.Services.AddSerilog();
    builder.Services.AddAdapters();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseScalar();
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseSerilogRequestLogging(opts =>
        opts.EnrichDiagnosticContext = SerilogConfig.EnrichFromRequest
    );
    app.UseExceptionHandler();
    // app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapGroupEndpoints();
    app.MapGet("/test", [EndpointSummary("測試")] () => "Hello World")
        .RequireAuthorization(PolicyNames.RequireAdminRole);
    app.MapGet(
        "/error",
        [EndpointSummary("測試錯誤")]
        () =>
        {
            throw new Exception("測試錯誤");
        }
    );
    app.MapGet(
        "/test-log",
        [EndpointSummary("測試 Log")]
        (ILogger<Program> logger) =>
        {
            logger.LogOperationStart("測試 Log", new { UserId = "1234567890" });
            logger.LogOperationEnd("測試 Log", new { UserId = "1234567890" });
            logger.LogOperationFailed("測試 Log", new Exception("測試錯誤"));
        }
    );
    app.MapGet(
        "/test-youbike",
        [EndpointSummary("測試 YouBike")]
        async (IYouBikeAdapter youBikeAdapter) =>
        {
            var youBikeImmediate = await youBikeAdapter.GetYouBikeImmediateAsync();
            return Results.Ok(youBikeImmediate);
        }
    );
    app.LogAppLifeTime();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
