---
name: api
description: API 專案（Minimal API 垂直切割）開發規範。新增或修改 Endpoint、OpenAPI metadata、Request/Response 模型與 Examples、資料驗證、Exception 處理、EF Core 或 Dapper 查詢與交易、第三方 API Adapter 時使用。
---

# API 開發規範

| 情境                                | 閱讀                             |
| ----------------------------------- | -------------------------------- |
| 撰寫 EF Core / Dapper 查詢、交易    | [database.md](database.md)       |
| 封裝第三方 API（Adapter、HttpClient） | [adapters.md](adapters.md)       |

## 垂直切割

採用 Minimal API 垂直切割，功能放在 `Modules` 資料夾下，一個 API 端點一個資料夾。

```text
Modules                                # 模組
└── Auth                               # 認證模組
    └── User                           # 使用者
        ├── Login                      # API 端點名稱
        │   ├── Examples.cs            # 範例
        │   ├── LoginEndpoint.cs       # 登入端點
        │   └── Models.cs              # 模型
        └── UserGroupEndpoints.cs      # 使用者群組端點
```

### MapGroupEndpoints

註冊所有 GroupEndpoints 的擴充方法。

```csharp
public static void MapGroupEndpoints(this WebApplication app)
{
    RouteGroupBuilder endpoints = app.MapGroup("/api");
    endpoints.MapUserGroupEndpoints();
}
```

### GroupEndpoints

- 命名規則 `[XXX]GroupEndpoints`（例：`UserGroupEndpoints`）
- 使用 `MapEndpoint<TEndpoint>` 擴充方法註冊端點

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

## Endpoint

- 實作 `IEndpoint` 介面
- `MapEndpoint` 的 URL 為資料夾名稱轉 camelCase：`InsertTodo` → `/insertTodo`
- OpenAPI metadata 一律用 **Attribute** 掛在 `Handler` 上，不使用 `.WithSummary()` 等擴充方法（GroupEndpoints 的 `.WithTags()` 除外）
- 回傳一律使用 `APIResponseHelper` 統一格式

| 操作             | 命名規則                        | URL                     |
| ---------------- | ------------------------------- | ----------------------- |
| 操作（非 CRUD）  | `[XXX]Endpoint`                 | `/login`                |
| 查詢（單筆）     | `Get[XXX]ByIdEndpoint`          | `/getTodoById`          |
| 查詢（多筆）     | `Get[XXX]ByQueryStringEndpoint` | `/getTodoByQueryString` |
| 新增             | `Insert[XXX]Endpoint`           | `/insertTodo`           |
| 更新（單筆）     | `Update[XXX]ByIdEndpoint`       | `/updateTodoById`       |
| 刪除（單筆）     | `Delete[XXX]ByIdEndpoint`       | `/deleteTodoById`       |

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

### OpenAPI Attributes

參考文件：<https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0&tabs=minimal-apis>

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
| 範例 Request  | `[RequestExample(exampleProviderType, name)]`                | 自定義           |
| 範例 Response | `[ResponseExample(statusCode, exampleProviderType, name)]`   | 自定義           |

範例的 `name` 寫成看得懂的情境：

- Request：`Admin`、`測試帳號`、`新增待辦事項 - 基本範例`、`新增待辦事項 - 僅標題`
- Response：`登入成功`、`帳號或密碼不正確`、`查詢待辦事項成功`、`查詢待辦事項空資料`

## Models 與資料驗證

- 檔案 `Models.cs`，類別命名 `[XXX]Request`、`[XXX]Response`、`[XXX]Dto`（例：`LoginRequest`、`LoginResponse`、`LoginDto`）
- 每個屬性撰寫 XML 註解
- 以 `DataAnnotations` 驗證 Query / Header / Request body，並以 `[Display]` 設定欄位顯示名稱

```csharp
public class LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Required]
    [Display(Name = "帳號")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;
}

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

驗證初始設定：

```csharp
builder.Services.AddValidation();
```

特定 Endpoint 不驗證（僅在使用者明確要求時使用）：

```csharp
app.MapPost("/login", Handler).DisableValidation();
```

## Examples

- 檔案 `Examples.cs`，實作 `IExampleProvider` 介面
- 類別命名 `[XXX]ReqEx_[說明]`、`[XXX]ResEx_[狀態碼]_[說明]`（例：`LoginReqEx_Admin`、`LoginResEx_Ok_LoginSuccess`、`LoginResEx_422_AccountOrPasswordIncorrect`）
- Response 範例回傳完整的 `APIResponse<T>`

```csharp
public class LoginReqEx_Admin : IExampleProvider
{
    public object GetExample() => new LoginRequest { UserId = "admin", Password = "p@ssw0rd" };
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

## Exception Handling

Endpoint 採用結果模式：錯誤以 `APIResponseHelper` 的回傳值表達，Handler 內不 throw exception。

```csharp
return APIResponseHelper.BusinessLogicError<LoginResponse>(message: "帳號或密碼不正確");

return APIResponseHelper.InternalServerError(
    exceptionDetails: new ExceptionDetails(
        type: ex.GetType().Name,
        title: "資料庫操作失敗",
        detail: ex.ToString(),
        requestId: httpContext.TraceIdentifier
    )
);
```
