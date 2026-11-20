# DataBase Optimization

## 資料夾結構

```text
Infrastructures/
└── Data/
    ├── Entities/             # 資料庫實體類別，需要知道資料欄位定義時先看這裡
    ├── XXXContext.cs         # EF Core 資料庫上下文
    └── XXXDapperContext.cs   # Dapper 連線工具
```

## 唯讀查詢使用 `AsNoTracking()`

```csharp
var user = await context
    .User.AsNoTracking()
    .FirstOrDefaultAsync(
        x => x.UserId == request.UserId && x.Password == request.Password,
        cancellationToken
    );
```

## Select 指定欄位

用 `Select` 投影成 DTO，只取需要的欄位。

```csharp
var users = await context
    .User.Select(x => new UserDto { UserId = x.UserId, UserName = x.UserName })
    .ToListAsync(cancellationToken);

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

## 避免 N+1 查詢

關聯資料一次批次取回，迴圈內不查資料庫。

```csharp
var todos = await context.Todos.AsNoTracking().ToListAsync(cancellationToken);
var userIds = todos.Select(t => t.UserId).Distinct().ToList();

var users = await context
    .Users.AsNoTracking()
    .Where(u => userIds.Contains(u.UserId))
    .ToDictionaryAsync(u => u.UserId, u => u, cancellationToken);

foreach (var todo in todos)
{
    todo.User = users.GetValueOrDefault(todo.UserId);
}
```

## EF Core + Dapper 混合使用

需要撰寫 SQL 時，使用 `context.Database` 上的 Dapper 擴充方法，與 EF Core 共用連線與交易。

```csharp
// 新增、更新、刪除
var affectedRows = await context.Database.DapperExecuteAsync(
    commandText: "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)",
    param: new { Name = "John Doe", Email = "john.doe@example.com" }
);

// 查詢
var users = await context.Database.DapperQueryAsync<User>(
    commandText: "SELECT * FROM Users WHERE Email = @Email",
    param: new { Email = "john.doe@example.com" }
);
```

## Transaction 交易管理

**預設使用 `SaveChangesAsync()`**：單次呼叫本身就是一個交易。

```csharp
context.Users.Add(newUser);
context.Roles.Add(newRole);
await context.SaveChangesAsync(cancellationToken);
```

**需要原子性時才使用 `BeginTransactionAsync()`**：多次 `SaveChangesAsync()`，或 EF Core 與 Dapper 混用，任一步失敗都要全部回滾。

```csharp
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

    await transaction.CommitAsync(cancellationToken);
    return APIResponseHelper.Ok("操作成功");
}
catch (Exception ex)
{
    await transaction.RollbackAsync(cancellationToken);
    return APIResponseHelper.InternalServerError(
        exceptionDetails: new ExceptionDetails(
            type: ex.GetType().Name,
            title: "資料庫操作失敗",
            detail: ex.ToString(),
            requestId: httpContext.TraceIdentifier
        )
    );
}
```
