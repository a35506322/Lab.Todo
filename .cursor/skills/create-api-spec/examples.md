# 完整範例：.NET Minimal API

**其他框架（Express / FastAPI / Spring Boot ...）同理**：找路由、找回應結構、找錯誤處理，套同一份模板。

## 輸入

### `src/TodoAPI/Modules/Todo/GetTodoById/GetTodoByIdEndpoint.cs`

```csharp
public class GetTodoByIdEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/getTodoById/{id:long}", Handler);

    [EndpointSummary("查詢待辦事項")]
    [EndpointDescription("根據 ID 查詢單筆待辦事項")]
    [ProducesResponseType<APIResponse<GetTodoByIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<GetTodoByIdResponse>>(StatusCodes.Status422UnprocessableEntity)]
    [ResponseExample(StatusCodes.Status200OK, typeof(GetTodoByIdResEx_Ok_Success), "查詢單筆待辦事項成功")]
    [ResponseExample(StatusCodes.Status422UnprocessableEntity, typeof(GetTodoByIdResEx_422_TodoNotFound), "找不到指定的待辦事項")]
    private static async Task<IResult> Handler(long id, LabContext context, CancellationToken ct)
    {
        var todo = await context.Todo.AsNoTracking()
            .Where(x => x.TodoId == id)
            .Select(x => new GetTodoByIdResponse { /* ... */ })
            .FirstOrDefaultAsync(ct);

        if (todo is null)
            return APIResponseHelper.BusinessLogicError<GetTodoByIdResponse>(message: "找不到指定的待辦事項");

        return APIResponseHelper.Ok(message: "查詢成功", data: todo);
    }
}
```

### `Models.cs`（同目錄）

```csharp
public class GetTodoByIdResponse
{
    /// <summary>待辦 ID</summary>
    public long TodoId { get; set; }

    /// <summary>待辦標題</summary>
    public string TodoTitle { get; set; } = null!;

    /// <summary>待辦內容</summary>
    public string? TodoContent { get; set; }

    /// <summary>是否完成 (Y/N)</summary>
    public string IsComplete { get; set; } = null!;

    /// <summary>完成時間</summary>
    public DateTime? CompleteTime { get; set; }

    /// <summary>新增時間</summary>
    public DateTime? AddTime { get; set; }

    /// <summary>新增者</summary>
    public string AddUserId { get; set; } = null!;
}
```

### `Examples.cs`（同目錄）

```csharp
public class GetTodoByIdResEx_Ok_Success : IExampleProvider
{
    public object GetExample() => new APIResponse<GetTodoByIdResponse>(
        Code.成功, "查詢成功",
        new GetTodoByIdResponse
        {
            TodoId = 1, TodoTitle = "測試待辦", TodoContent = "測試內容",
            IsComplete = "N", CompleteTime = null,
            AddTime = DateTime.Now, AddUserId = "testuser"
        }
    );
}

public class GetTodoByIdResEx_422_TodoNotFound : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<GetTodoByIdResponse>(Code.商業邏輯錯誤, "找不到指定的待辦事項", default);
}
```

### `TodoGroupEndpoints.cs`（上層 group）

```csharp
RouteGroupBuilder todoEndpoints = app.MapGroup("/todo")
    .WithTags("Todo")
    .RequireAuthorization(PolicyNames.RequireAdminRole);
```

### `APIExtension.cs`（root group）

```csharp
RouteGroupBuilder endpoints = app.MapGroup("/api");
```

### 全域 ExceptionHandler

未捕捉的例外 → `InternalServerExceptionHandler` 包成 `APIResponse<object>` 回 500

---

## 輸出：`docs/api/get-todo-by-id.md`

````markdown
# 查詢待辦事項

## 基本資訊

| 項目 | 內容 |
|---|---|
| URL | `/api/todo/getTodoById/{id}` |
| Method | GET |
| 描述 | 根據 ID 查詢單筆待辦事項 |
| 認證 | 需要 JWT，角色：Admin |

## Request

### Path Parameters

| 欄位 | 型別 | 必填 | 說明 | 範例 |
|---|---|---|---|---|
| id | long | Y | 待辦 ID | 1 |

### Query Parameters

（無）

### Headers

| 欄位 | 必填 | 說明 |
|---|---|---|
| Authorization | Y | Bearer {token} |

### Body

（無）

## Response

### 成功

#### 200 OK

**欄位說明**

| 欄位 | 型別 | 說明 | 範例 |
|---|---|---|---|
| code | int | 狀態碼（2000 = 成功） | 2000 |
| message | string | 訊息 | "查詢成功" |
| data.todoId | long | 待辦 ID | 1 |
| data.todoTitle | string | 待辦標題 | "測試待辦" |
| data.todoContent | string? | 待辦內容 | "測試內容" |
| data.isComplete | string | 是否完成（Y/N） | "N" |
| data.completeTime | datetime? | 完成時間 | null |
| data.addTime | datetime? | 新增時間 | "2026-05-23T10:00:00" |
| data.addUserId | string | 新增者 | "testuser" |
| validationErrors | object? | 驗證錯誤 | null |
| exceptionDetails | object? | 例外詳情 | null |
| traceId | string | 追蹤 ID | "00-abc123-xyz-00" |

**Example**

```json
{
  "code": 2000,
  "message": "查詢成功",
  "data": {
    "todoId": 1,
    "todoTitle": "測試待辦",
    "todoContent": "測試內容",
    "isComplete": "N",
    "completeTime": null,
    "addTime": "2026-05-23T10:00:00",
    "addUserId": "testuser"
  },
  "validationErrors": null,
  "exceptionDetails": null,
  "traceId": "00-abc123-xyz-00"
}
```

### 錯誤

| HTTP Code | 觸發條件 | Example |
|---|---|---|
| 401 | 未提供或 JWT 失效 | `{ "code": 4001, "message": "授權失敗", "data": null, "traceId": "..." }` |
| 403 | 角色非 Admin | `{ "code": 4003, "message": "驗證權限失敗", "data": null, "traceId": "..." }` |
| 422 | 找不到指定的待辦事項 | `{ "code": 4022, "message": "找不到指定的待辦事項", "data": null, "traceId": "..." }` |
| 500 | 未捕捉的例外（全域 ExceptionHandler 處理） | `{ "code": 5000, "message": "程式內部錯誤", "exceptionDetails": { "type": "...", "title": "...", "detail": "...", "requestId": "..." }, "traceId": "..." }` |
````

---

## 從這個範例可以學到的推導模式

| 觀察 | 推導 |
|---|---|
| `app.MapGet("/getTodoById/{id:long}", ...)` | Method = GET，path = `/getTodoById/{id}`，path param `id` 型別 long |
| 上層 `MapGroup("/todo")` + root `MapGroup("/api")` | 完整 URL = `/api/todo/getTodoById/{id}` |
| 上層 `RequireAuthorization(PolicyNames.RequireAdminRole)` | 認證 = 需要 JWT + Admin 角色 → 衍生 401 / 403 錯誤 |
| `[ProducesResponseType<APIResponse<T>>(Status200OK)]` | 成功 200，包在統一 envelope |
| Handler 內 `return APIResponseHelper.BusinessLogicError(...)` | 衍生 422 錯誤，訊息取自呼叫參數 |
| 專案有全域 ExceptionHandler 包 500 | 必加 500 錯誤行 |
| `IExampleProvider` 提供物件範例 | JSON example 從這裡轉，欄位序列化規則依專案 JSON 設定（如 camelCase、enum 是數字還是字串） |
