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

資料夾結構

-   Entities 資料庫實體類別，**如需知道資料欄位定義可參考此資料夾**
-   XXXContext EF Core 資料庫上下文
-   XXDapperContext 資料庫 Dapper 連線工具

```text
├── Infrastructures/
│   ├── Data/
│   │   ├── Entities/  # 資料庫實體類別
│   │   ├── XXXContext.cs  # XXX 資料庫上下文
│   │   ├── XXXDapperContext.cs  # XXX Dapper 連線工具
```

1. EF Core 查詢資料時使用 `AsNoTracking()` 避免不必要的資料庫查詢

```csharp
var user = await context
    .User.AsNoTracking()
    .FirstOrDefaultAsync(
        x => x.UserId == request.UserId && x.Password == request.Password,
        cancellationToken
    );
```

2. EF Core + Dapper 混合使用

如需**撰寫 SQL 操作**請使用 EF Core + Dapper 的擴充方法，方便管理上下文和交易

```csharp

✅ 引用 EF Core + Dapper 擴充方法
✅ CRUD 操作
var result = await context.Database.DapperExecuteAsync(
    commandText:"INSERT INTO Users (Name, Email) VALUES (@Name, @Email)",
    param: new { Name = "John Doe", Email = "john.doe@example.com" }
);

✅ 查詢操作
var result = await context.Database.DapperQueryAsync<User>(
    commandText: "SELECT * FROM Users WHERE Email = @Email",
    param: new { Email = "john.doe@example.com" }
)
```

3. 不要 N+1 查詢問題

```csharp
❌ N+1
var todos = await context.Todos.ToListAsync(cancellationToken);
foreach (var todo in todos)
{
    todo.User = await context.Users.FirstOrDefaultAsync(u => u.UserId == todo.UserId);
}

✅ 兩次查詢：先取 Todo，再取對應的 User
var todos = await context.Todos.AsNoTracking().ToListAsync(cancellationToken);
var userIds = todos.Select(t => t.UserId).Distinct().ToList();

var users = await context.Users
    .AsNoTracking()
    .Where(u => userIds.Contains(u.UserId))
    .ToDictionaryAsync(u => u.UserId, u => u, cancellationToken);

foreach (var todo in todos)
{
    todo.User = users.GetValueOrDefault(todo.UserId);
}
```

4. Select 指定欄位

```csharp
❌ BAD: 使用 Select *
var users = await context.User.ToListAsync();

✅ 使用 Select 指定欄位
var users = await context.User.Select(x => new UserDto{ UserId = x.UserId, UserName = x.UserName }).ToListAsync();

✅ 建立 DTO 類別
public class UserDto
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}
```

4. Transaction 交易管理

**預設使用 SaveChangesAsync()**

```csharp
    context.Users.Add(newUser);
    context.Roles.Add(newRole);

    ✅ 使用 `SaveChangesAsync()` 保存變更
    await context.SaveChangesAsync(cancellationToken);
```

**使用 `BeginTransactionAsync()` 明確管理交易**

```csharp

✅ 使用 `BeginTransactionAsync()` 明確管理交易
using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

try
{
    // EF Core 操作
    context.User.Add(newUser);
    await context.SaveChangesAsync(cancellationToken);

    // Dapper 操作（自動參與同一個交易）
    await context.Database.DapperExecuteAsync(
        commandText: "UPDATE Role SET UserCount = UserCount + 1 WHERE RoleId = @RoleId",
        param: new { RoleId = newUser.RoleId }
    );

    // 再次 EF Core 操作
    context.ProcessLogs.Add(newProcessLog);
    await context.SaveChangesAsync(cancellationToken);

    ✅ 提交交易
    await transaction.CommitAsync(cancellationToken);

    return APIResponseHelper.Ok("操作成功");
}
catch (Exception ex)
{
    ✅ 發生錯誤時回滾
    await transaction.RollbackAsync(cancellationToken);
    return APIResponseHelper.InternalServerError(exceptionDetails: new ExceptionDetails(type: ex.GetType().Name, title: "資料庫操作失敗", detail: ex.ToString(), requestId: httpContext.TraceIdentifier));
}
```

**多個 SaveChangesAsync 需要原子性**

```csharp
using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

try
{
    // 第一個操作
    context.Users.Add(newUser);
    await context.SaveChangesAsync(cancellationToken);

    // 第二個操作（如果失敗，第一個也要回滾）
    context.Roles.Add(newRole);
    await context.SaveChangesAsync(cancellationToken);

    await transaction.CommitAsync(cancellationToken);
    return APIResponseHelper.Ok("操作成功");
}
catch (Exception ex)
{
    await transaction.RollbackAsync(cancellationToken);
    return APIResponseHelper.InternalServerError(exceptionDetails: new ExceptionDetails(type: ex.GetType().Name, title: "資料庫操作失敗", detail: ex.ToString(), requestId: httpContext.TraceIdentifier));
}
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
