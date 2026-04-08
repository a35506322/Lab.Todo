using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TodoAPI.Infrastructures.OpenAPI;

/// <summary>
/// 將 enum 的名稱與實際數值追加到 OpenAPI schema description，方便文件直接查看對照表。
/// </summary>
public class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    /// <summary>
    /// 針對 enum schema 補上名稱與數值說明。
    /// </summary>
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var targetType =
            Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
        if (!targetType.IsEnum)
        {
            return Task.CompletedTask;
        }

        var valueNodes = new List<JsonNode?>();
        foreach (var name in Enum.GetNames(targetType))
        {
            var field = targetType.GetField(name, BindingFlags.Public | BindingFlags.Static);
            var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            var intVal = Convert.ToInt32(Enum.Parse(targetType, name));
            var part = string.IsNullOrWhiteSpace(desc) ? $"{intVal} = {name}" : $"{intVal} = {name}（{desc}）";
            valueNodes.Add(JsonValue.Create(part));
        }
        schema.Enum = valueNodes;

        return Task.CompletedTask;
    }
}
