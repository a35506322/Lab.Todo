using System.Collections.Frozen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TodoAPI.Infrastructures.OpenAPI;

// https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/openapi/customize-openapi?view=aspnetcore-9.0
public class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider
) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (!authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            return;

        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
            },
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        // 為需要認證的 endpoints 加上 Bearer 為必輸
        var authorizedEndpoints = GetAuthorizedEndpoints(context.DescriptionGroups);
        foreach (var (path, pathItem) in document.Paths ?? [])
        {
            foreach (var (operationType, operation) in pathItem.Operations ?? [])
            {
                if (!authorizedEndpoints.Contains((NormalizePath(path), operationType)))
                    continue;

                operation.Security ??= [];
                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
                    }
                );
            }
        }
    }

    private static FrozenSet<(string Path, HttpMethod Method)> GetAuthorizedEndpoints(
        IReadOnlyList<ApiDescriptionGroup>? descriptionGroups
    )
    {
        if (descriptionGroups is null or [])
            return FrozenSet<(string, HttpMethod)>.Empty;

        var authorized = new HashSet<(string Path, HttpMethod Method)>();

        foreach (var group in descriptionGroups)
        {
            foreach (var api in group.Items)
            {
                if (!RequiresAuthorization(api))
                    continue;

                if (ParseHttpMethod(api.HttpMethod) is { } method)
                    authorized.Add((NormalizePath(api.RelativePath), method));
            }
        }

        return authorized.ToFrozenSet();
    }

    private static bool RequiresAuthorization(ApiDescription api)
    {
        var metadata = api.ActionDescriptor?.EndpointMetadata;
        if (metadata is null)
            return false;

        var hasAllowAnonymous = metadata.OfType<IAllowAnonymous>().Any();
        if (hasAllowAnonymous)
            return false;

        return metadata.OfType<IAuthorizeData>().Any();
    }

    private static string NormalizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return "/";
        return path.StartsWith('/') ? path : "/" + path;
    }

    private static HttpMethod? ParseHttpMethod(string? httpMethod) =>
        httpMethod?.ToUpperInvariant() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            _ => null,
        };
}
