var builder = WebApplication.CreateBuilder(args);

// Tips: 我使用 Cursor 開啟中斷點偵錯時，無法正確取得 ContentRootPath，所以需要手動設定
// var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "";
// var contentRoot = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", ".."));
// var builder = WebApplication.CreateBuilder(
//     new WebApplicationOptions { ContentRootPath = contentRoot }
// );

builder.Services.AddOpenAPI();
builder.Services.AddEFCore(builder.Configuration);
builder.Services.AddJWT(builder.Configuration);
builder.Services.AddDI();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSecurity();
builder.Services.AddExceptionHandlerConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseScalar();
}

app.UseHttpsRedirection();
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

app.Run();
