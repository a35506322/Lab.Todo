---
name: api-patterns
description: 適用於 API 專案最佳實踐模式，裡面包含 API 垂直切割, DataBase Optimization,、Log、ExceptionHandler、Endpoint Open API 等
---

# API 開發規範

適用於 API 專案最佳實踐模式，裡面包含`API 垂直切割`、`DataBase Optimization`、`Log`、`ExceptionHandler`、`Endpoint Open API`等，只要在專案中需要使用到相關功能，請毫不吝嗇的閱讀並理解技能內容

## 垂直切割

採用 Minimal API 垂直切割，並將相關功能放在 `Modules` 資料夾下

資料夾結構

```
Modules # 模組
├── Auth # 認證模組
│   └── User # 使用者
│       ├── Login # 登入
│       │   ├── Examples.cs # 範例
│       │   ├── LoginEndpoint.cs # 登入端點
│       │   └── Models.cs # 模型
│       └── UserGroupEndpoints.cs # 使用者群組端點
```

GroupEndpoints

-   命名規則 `[XXX]GroupEndpoints` (ex: `UserGroupEndpoints`)

```csharp
public static class UserGroupEndpoints
{
    public static void MapUserGroupEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder userEndpoints = app.MapGroup("/user").WithTags("User");
        userEndpoints.MapEndpoint<LoginEndpoint>();
    }
}

```

Endpoint

-   繼承 `IEndpoint` 介面並實作
-   套用 Open API 相關 Attributes
-   回傳 Response 使用 `APIResponseHelper` 統一回傳格式
-   檔案名稱 `[XXX]Endpoint` (ex: `LoginEndpoint`)

```csharp
public class LoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app.MapPost("/login", Handler);

    [EndpointName("Login")]
    [EndpointSummary("登入")]
    [EndpointDescription("登入成功後回傳 Token")]
    [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status422UnprocessableEntity)]
    [RequestExample(typeof(LoginReqEx_Admin), "Admin")]
    [RequestExample(typeof(LoginReqEx_Demo), "測試帳號")]
    [ResponseExample(StatusCodes.Status200OK, typeof(LoginResEx_Ok_LoginSuccess), "登入成功")]
    [ResponseExample(
        StatusCodes.Status422UnprocessableEntity,
        typeof(LoginResEx_422_AccountOrPasswordIncorrect),
        "帳號或密碼不正確"
    )]
    private static async Task<IResult> Handler(
        LoginRequest request,
        LabContext context,
        IJWTHelper jwtHelper,
        CancellationToken cancellationToken
    )
    {
        var user = await context
            .User.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.Password == request.Password,
                cancellationToken
            );

        if (user is null)
        {
            return APIResponseHelper.BusinessLogicError<LoginResponse>(message: "帳號或密碼不正確");
        }

        var token = jwtHelper.GenerateToken(
            userId: user.UserId,
            roles: new List<string> { user.Role }
        );
        var expiresIn = jwtHelper.GetExpiresIn();
        return APIResponseHelper.Ok(
            message: "登入成功",
            data: new LoginResponse { Token = token, ExpiresIn = expiresIn }
        );
    }
}
```

Models

-   使用 `DataAnnotations` 驗證 Request Body
-   使用 `Display` 設定 Display Name
-   撰寫 XML 註解
-   檔案名稱 `[XXX]Request`、`[XXX]Response`、`[XXX]Dto` (ex: `LoginRequest`、`LoginResponse`、`LoginDto`)

```csharp
public class LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
};

public class LoginResponse
{
    /// <summary>
    /// 登入 Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token 時效 (單位: 分鐘)
    /// </summary>
    public int ExpiresIn { get; set; } = 0;
}
```

Examples

-   繼承 `IExampleProvider` 介面並實作
-   檔案名稱 `[XXX]RequestEx_[說明]`、`[XXX]ResponseEx_[說明]`、`[XXX]DtoEx_[說明]` (ex: `LoginReqEx_Admin_Admin`、`LoginResEx_Ok_LoginSuccess_登入成功`、`LoginDtoEx_Ok_LoginSuccess_登入成功空資料`)

```csharp

public class LoginReqEx_Admin : IExampleProvider
{
    public object GetExample() => new LoginRequest { UserId = "admin", Password = "p@ssw0rd" };
}

public class LoginReqEx_Demo : IExampleProvider
{
    public object GetExample() => new LoginRequest { UserId = "demo", Password = "123456" };
}

public class LoginResEx_Ok_LoginSuccess : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<LoginResponse>(
            Code.成功,
            "登入成功",
            new LoginResponse { Token = "1234567890", ExpiresIn = 60 }
        );
}

public class LoginResEx_422_AccountOrPasswordIncorrect : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<LoginResponse>(Code.商業邏輯錯誤, "帳號或密碼不正確", default);
}

```

## 資料驗證

初始設定

```csharp
builder.Services.AddValidation();
```

使用 `DataAnnotations` 驗證 Query / Header / Request body

```csharp
[Required]
[Display(Name = "帳號")]
public string UserId { get; set; } = string.Empty;

[Required]
[Display(Name = "密碼")]
public string Password { get; set; } = string.Empty;
```

特定 Endpoint 不驗證 (需使用者特別說明)

```csharp
app.MapPost("/login", Handler).DisableValidation();
```

## DataBase Optimization

1. EF Core 查詢資料時使用 `AsNoTracking()` 避免不必要的資料庫查詢

```csharp
var user = await context
    .User.AsNoTracking()
    .FirstOrDefaultAsync(
        x => x.UserId == request.UserId && x.Password == request.Password,
        cancellationToken
    );
```

## Endpoint Open API

**⚠️ 此設定是在撰寫 Endpoint 時設定，不是在撰寫 Group Endpoint 時設定**

參考文件: https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0&tabs=minimal-apis

在 Minimal API 中設定 OpenAPI metadata 有兩種方式：

1. **Attribute（標籤）** - 放在 Handler 方法上(✅ 推薦使用)
2. **Extension Method** - 如 `.WithTags()`、`.WithSummary()` 等（❌ 不要使用）

```csharp
[EndpointName("Login")]
[EndpointSummary("登入")]
[EndpointDescription("登入成功後回傳 Token")]
[ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status200OK)]
[ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status422UnprocessableEntity)]
[RequestExample(typeof(LoginReqEx_Admin), "Admin")]
[RequestExample(typeof(LoginReqEx_Demo), "測試帳號")]
[ResponseExample(StatusCodes.Status200OK, typeof(LoginResEx_Ok_LoginSuccess), "登入成功")]
[ResponseExample(
    StatusCodes.Status422UnprocessableEntity,
    typeof(LoginResEx_422_AccountOrPasswordIncorrect),
    "帳號或密碼不正確"
)]
private static async Task<IResult> Handler(
    LoginRequest request,
    LabContext context,
    CancellationToken cancellationToken
)
```

官方文件可用的 Attributes 對照表

| Metadata     | Attribute                                                    | 說明             |
| ------------ | ------------------------------------------------------------ | ---------------- |
| operationId  | `[EndpointName]`                                             | 操作的唯一識別碼 |
| summary      | `[EndpointSummary]`                                          | 摘要（短描述）   |
| description  | `[EndpointDescription]`                                      | 詳細描述         |
| tags         | `[Tags]`                                                     | 分類標籤         |
| responses    | `[ProducesResponseType]`                                     | 回應類型         |
| request body | `[FromBody]` + `[Description]`                               | 請求內容描述     |
| parameter    | `[FromQuery]`/`[FromRoute]`/`[FromHeader]` + `[Description]` | 參數描述         |
| 排除         | `[ExcludeFromDescription]`                                   | 從文件排除       |

自定義 Attributes 對照表

| Metadata            | Attribute           | 說明               |
| ------------------- | ------------------- | ------------------ |
| exampleProviderType | `[RequestExample]`  | 範例 Request 類型  |
| name                | `[RequestExample]`  | 範例顯示名稱       |
| statusCode          | `[ResponseExample]` | HTTP 狀態碼        |
| exampleProviderType | `[ResponseExample]` | 範例 Response 類型 |
| name                | `[ResponseExample]` | 範例顯示名稱       |

## Exception Handling

在 Endpoint 中不要 throw exception，而是使用結果模式，統一使用 `APIResponseHelper` 統一回傳格式

```csharp
❌ BAD: 使用 throw exception
throw new Exception("測試錯誤");

✅ GOOD: 使用 APIResponseHelper 統一回傳格式
return APIResponseHelper.InternalServerError(exceptionDetails: new ExceptionDetails(type: "Exception", title: "測試錯誤", detail: "Exception", requestId: "1234567890"));
return APIResponseHelper.BusinessLogicError<LoginResponse>(message: "帳號或密碼不正確");

```
