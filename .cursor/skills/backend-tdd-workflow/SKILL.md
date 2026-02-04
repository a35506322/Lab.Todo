---
name: backend-tdd-workflow
description: 在編寫後端專案新功能、修復錯誤或重構程式碼時運用此技能。強制執行測試驅動開發，包含單元測試與整合測試。
---

# TDD 工作流程規範

在編寫後端新功能、修復錯誤或重構程式碼時運用此技能。強制執行測試驅動開發，包含單元測試與整合測試。

## TDD 使用場景

-   撰寫/修改新功能或新特性
-   修復錯誤或問題
-   重構現有程式碼
-   新增/修改 API 端點

## TDD 核心原則

1.  先寫測試在寫程式碼
2.  單元測試項目應該僅驗證單一行為，整合測試項目應該驗證完整流程
3.  測試名稱應該描述性，說明測試內容
4.  測試結構應該清晰，使用 Arrange-Act-Assert 模式
5.  測試應該隔離外部依賴項，使用 Mock 模擬
6.  測試應該測試邊界情況，包括空值、未定義、空值、大型數值等
7.  測試應該保持測試速度快，單元測試每項 < 50 毫秒

## 測試類型

**Unit Tests**

-   輔助工具
-   純函數

**Integration Tests**

-   驗證完整流程
-   驗證 API 端點
-   驗證資料庫操作
-   驗證服務互動
-   驗證外部 API 呼叫

## TDD 工作流程步驟

**步驟一：撰寫用戶旅程測試**

```
作為一個[使用者角色]，我希望能[使用者行為]，以便[使用者受益]。

Example:
作為一名使用者，我想要登入系統，這樣我就可以使用系統。
```

**步驟二：生成測試案例**

```csharp
[TestClass]
public class LoginEndpointTests
{
    [TestMethod]
    public async Task Handler_ValidCredentials_ReturnsOk()
    {
        ....
    }

    [TestMethod]
    public async Task Handler_InvalidCredentials_ReturnsBusinessLogicError()
    {
        ....
    }

    [TestMethod]
    public async Task Handler_WrongPassword_ReturnsBusinessLogicError()
    {
        ....

    }
}

```

**步驟 3：執行測試（它們應該會失敗）**

```csharp
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_ValidCredentials_ReturnsOk"
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_InvalidCredentials_ReturnsBusinessLogicError"
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_WrongPassword_ReturnsBusinessLogicError"
```

**步驟 4：編寫程式碼以通過測試**

```csharp
public class LoginEndpoint
{
    public static async Task<IResult> Handler(LoginRequest request)
    {
        return APIResponseHelper.Ok(message: "登入成功");
    }
}
```

**步驟 5：執行測試（它們應該會成功）**

```csharp
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_ValidCredentials_ReturnsOk"
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_InvalidCredentials_ReturnsBusinessLogicError"
dotnet test --filter "FullyQualifiedName=TodoAPI.UnitTest.EndpointTests.LoginEndpointTests.Handler_WrongPassword_ReturnsBusinessLogicError"
```

**步驟 6：重構程式碼**

-   移除重複內容
-   改善命名
-   優化效能
-   提升可讀性

## 撰寫測試 Skill

請查閱相關技能並學習

-   [mstest-unit-test](../mstest-unit-test/SKILL.md)
-   [integration-test](../integration-test/SKILL.md)

## 成功指標

-   所有測試皆通過（綠色）
