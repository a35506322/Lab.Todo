namespace TodoAPI.Infrastructures.OpenAPI;

public static class OpenAPIConfig
{
    public static void AddOpenAPI(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(
                (document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Lab.TodoAPI",
                        Version = "v1",
                        Description = "Lab.TodoAPI API",
                    };
                    return Task.CompletedTask;
                }
            );

            // 註冊 Example Transformer，讓標了 RequestExampleAttribute 和 ResponseExampleAttribute 的 endpoint 自動在 OpenAPI 顯示範例.
            options.AddOperationTransformer(ExampleOperationTransformer.TransformAsync);

            // 註冊 JWT Security Scheme，讓 OpenAPI 顯示 JWT 認證.
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }
}
