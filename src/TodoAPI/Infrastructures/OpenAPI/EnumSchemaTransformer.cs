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
            return Task.CompletedTask;

        var enumDescriptions = Enum.GetValues(enumType: targetType)
            .Cast<object>()
            .Select(value => $"{Enum.GetName(targetType, value)} = {Convert.ToInt64(value)}")
            .ToList();
        if (enumDescriptions.Count == 0)
            return Task.CompletedTask;

        var enumDescriptionBlock = $"可用列舉值：<br/>{string.Join("<br/>", enumDescriptions)}";
        schema.Description = string.IsNullOrWhiteSpace(value: schema.Description)
            ? enumDescriptionBlock
            : $"{schema.Description}<br/>{enumDescriptionBlock}";

        return Task.CompletedTask;
    }
}
