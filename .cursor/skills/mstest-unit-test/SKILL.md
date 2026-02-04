---
name: mstest-unit-test
description: 適用於 MSTest 單元測試最佳實踐模式，裡面包含 MSTest 單元測試、NSubstitute Mock、Entity Framework Core InMemory 等
---

# MSTest 單元測試規範

適用於 MSTest 單元測試最佳實踐模式，裡面包含`MSTest 單元測試`、`NSubstitute Mock`、`Entity Framework Core InMemory`等

## 使用場景

-   撰寫/修改輔助工具
-   撰寫/修改純函數

**note:Endpoint Handler 方法 不要寫單元測試，改寫整合測試**

## 測試工具

-   MSTest
-   NSubstitute (Mock 工具)
-   Entity Framework Core InMemory (In-Memory 資料庫)
-   UnitTesting.Assert (斷言工具)

## MSTest 單元測試

主要測試引用專案的 Adapter、Helper、Repository 等類別的方法

### 資料夾結構

可用工具類別建立測試資料夾

```
├── [XXX]Test # [XXX] 常用工具測試
│   └── [XXX]Tests.cs # [XXX] 常用工具測試
```

### 命名規範

| 類型     | 命名規範                   |
| -------- | -------------------------- |
| 測試檔名 | [XXX]ToolTests.cs          |
| 測試方法 | 方法名\_測試情境\_預期結果 |

### 範例

```csharp
[TestClass]
public class YouBikeAdapterTests
{
    private HttpClient _httpClient = null!;
    private IConfiguration _configuration = null!;
    private YouBikeAdapter _adapter = null!;
    private HttpMessageHandler _messageHandler = null!;

    [TestInitialize]
    public void Setup()
    {
        // Mock Configuration
        _configuration = Substitute.For<IConfiguration>();
        _configuration.GetValue<string>("YouBike:BaseUrl").Returns("https://api.example.com/");

        // 建立 Mock HttpMessageHandler
        _messageHandler = Substitute.For<HttpMessageHandler>();

        // 建立 HttpClient (實際使用時需要 Mock HttpMessageHandler)
        // 這裡示範如何測試，實際可能需要使用 HttpClientFactory 或 Mock HttpMessageHandler
    }

    [TestMethod]
    public async Task GetYouBikeImmediateAsync_ReturnsData_WhenApiSucceeds()
    {
        // Arrange
        var expectedData = new List<YouBikeImmediateDto>
        {
            new YouBikeImmediateDto
            {
                Sno = "0001",
                Sna = "測試站點",
                Tot = 30,
                Sbi = 10,
                Bemp = 20
            }
        };

        // 注意：實際測試 HttpClient 時，可用 RichardSzalay.MockHttp
        // 這裡示範測試邏輯結構

        // Act
         var result = await _adapter.GetYouBikeImmediateAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }
}
```
