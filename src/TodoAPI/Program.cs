var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenAPI();
builder.Services.AddEFCore(builder.Configuration);
builder.Services.AddJWT(builder.Configuration);
builder.Services.AddDI();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSecurity();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseScalar();
}

app.UseHttpsRedirection();

// app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGroupEndpoints();
app.MapGet("/", [EndpointSummary("測試")] () => "Hello World")
    .RequireAuthorization(PolicyNames.RequireAdminRole);

app.Run();
