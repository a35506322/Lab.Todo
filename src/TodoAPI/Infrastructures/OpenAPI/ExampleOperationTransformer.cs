using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TodoAPI.Infrastructures.OpenAPI;

/// <summary>
/// 讀取 Handler 上的 <see cref="ResponseExampleAttribute"/>、<see cref="RequestExampleAttribute"/>，
/// 將範例填進 OpenAPI operation 的 request body 與 response.
/// 同一個 status / request 多個具名範例時會填 <c>examples</c>，Scalar 會顯示為下拉選單.
/// </summary>
public static class ExampleOperationTransformer
{
    public static Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var methodInfo = GetHandlerMethodInfo(context);
        if (methodInfo is null)
            return Task.CompletedTask;

        ApplyRequestExamples(operation, methodInfo);
        ApplyResponseExamples(operation, methodInfo);

        return Task.CompletedTask;
    }

    private static void ApplyRequestExamples(OpenApiOperation operation, MethodInfo methodInfo)
    {
        var attributes = methodInfo.GetCustomAttributes<RequestExampleAttribute>().ToList();
        if (attributes.Count == 0)
            return;

        if (operation.RequestBody is null)
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType(),
                },
            };
        if (
            operation.RequestBody.Content is null
            || !operation.RequestBody.Content.TryGetValue("application/json", out var mediaType)
        )
            return;

        var useMultiple = attributes.Count > 1 || !string.IsNullOrWhiteSpace(attributes[0].Name);
        if (useMultiple)
        {
            if (mediaType.Examples is null)
                mediaType.Examples = new Dictionary<string, IOpenApiExample>();
            foreach (var attr in attributes)
            {
                try
                {
                    var provider = (IExampleProvider)
                        Activator.CreateInstance(attr.ExampleProviderType)!;
                    var example = provider.GetExample();
                    var value = JsonNode.Parse(JsonSerializer.Serialize(example))!;
                    var key = GetRequestExampleKey(attr);
                    mediaType.Examples[key] = new OpenApiExample
                    {
                        Summary = attr.Name ?? key,
                        Value = value,
                    };
                }
                catch
                {
                    // 建立失敗就跳過
                }
            }
            mediaType.Example = null;
        }
        else
        {
            try
            {
                var attr = attributes[0];
                var provider = (IExampleProvider)
                    Activator.CreateInstance(attr.ExampleProviderType)!;
                var example = provider.GetExample();
                mediaType.Example = JsonNode.Parse(JsonSerializer.Serialize(example))!;
            }
            catch
            {
                // 建立失敗就跳過
            }
        }
    }

    private static string GetRequestExampleKey(RequestExampleAttribute attr)
    {
        if (!string.IsNullOrWhiteSpace(attr.Name))
            return attr.Name.Trim();
        var typeName = attr.ExampleProviderType.Name;
        if (typeName.EndsWith("Example", StringComparison.OrdinalIgnoreCase) && typeName.Length > 7)
            typeName = typeName[..^7];
        return typeName;
    }

    private static void ApplyResponseExamples(OpenApiOperation operation, MethodInfo methodInfo)
    {
        var attributes = methodInfo.GetCustomAttributes<ResponseExampleAttribute>().ToList();
        if (attributes.Count == 0)
            return;

        foreach (var group in attributes.GroupBy(a => a.StatusCode))
        {
            var statusKey = group.Key.ToString();
            if (
                operation.Responses is null
                || !operation.Responses.TryGetValue(statusKey, out var response)
                || response.Content is null
            )
                continue;
            if (!response.Content.TryGetValue("application/json", out var mediaType))
                continue;

            var list = group.ToList();
            var useMultiple = list.Count > 1 || list[0].Name is { } n && n.Length > 0;

            if (useMultiple)
            {
                if (mediaType.Examples is null)
                    mediaType.Examples = new Dictionary<string, IOpenApiExample>();
                foreach (var attr in list)
                {
                    try
                    {
                        var provider = (IExampleProvider)
                            Activator.CreateInstance(attr.ExampleProviderType)!;
                        var example = provider.GetExample();
                        var value = JsonNode.Parse(JsonSerializer.Serialize(example))!;
                        var key = GetExampleKey(attr);
                        mediaType.Examples[key] = new OpenApiExample
                        {
                            Summary = attr.Name ?? key,
                            Value = value,
                        };
                    }
                    catch
                    {
                        // 建立失敗就跳過該 example
                    }
                }
                mediaType.Example = null;
            }
            else
            {
                try
                {
                    var attr = list[0];
                    var provider = (IExampleProvider)
                        Activator.CreateInstance(attr.ExampleProviderType)!;
                    var example = provider.GetExample();
                    mediaType.Example = JsonNode.Parse(JsonSerializer.Serialize(example))!;
                }
                catch
                {
                    // 建立失敗就跳過
                }
            }
        }
    }

    private static string GetExampleKey(ResponseExampleAttribute attr)
    {
        if (!string.IsNullOrWhiteSpace(attr.Name))
            return attr.Name.Trim();
        var typeName = attr.ExampleProviderType.Name;
        return typeName;
    }

    /// <summary>
    /// Minimal API 的 Handler 若為 method group，通常會出現在 EndpointMetadata 的 MethodInfo.
    /// </summary>
    private static MethodInfo? GetHandlerMethodInfo(OpenApiOperationTransformerContext context)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        return metadata?.OfType<MethodInfo>().FirstOrDefault();
    }
}
