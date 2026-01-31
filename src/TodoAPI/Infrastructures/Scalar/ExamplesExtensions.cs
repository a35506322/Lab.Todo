using Microsoft.AspNetCore.OpenApi;

namespace TodoAPI.Infrastructures.Scalar;

/// <summary>
/// 註冊 Example Transformer，讓標了 <see cref="RequestExampleAttribute"/>、<see cref="ResponseExampleAttribute"/> 的 endpoint 自動在 OpenAPI 顯示範例.
/// </summary>
public static class ExamplesExtensions
{
    public static OpenApiOptions AddExamples(this OpenApiOptions options)
    {
        options.AddOperationTransformer(ExampleOperationTransformer.TransformAsync);
        return options;
    }
}
