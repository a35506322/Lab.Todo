---
name: mstest-unit-test
description: 適用於 MSTest 單元測試最佳實踐模式，裡面包含 MSTest 單元測試、NSubstitute Mock、Entity Framework Core InMemory 等
---

# MSTest 單元測試規範

適用於 MSTest 單元測試最佳實踐模式，裡面包含`MSTest 單元測試`、`NSubstitute Mock`、`Entity Framework Core InMemory`等

## 測試工具

-   MSTest
-   NSubstitute (Mock 工具)
-   Entity Framework Core InMemory (In-Memory 資料庫)
-   UnitTesting.Assert (斷言工具)

## Endpoint MSTest 單元測試

主要測試 Endpoint 的 Handler 方法

### 資料夾結構

```
├── EndpointTest # Endpoint 測試
│   └── [XXX]EndpointTests.cs # [XXX] Endpoint 測試
```

### 命名規範

| 類型     | 命名規範                   |
| -------- | -------------------------- |
| 測試檔名 | [XXX]EndpointTests.cs      |
| 測試方法 | 方法名\_測試情境\_預期結果 |

### 範例

```csharp
namespace TodoAPI.UnitTests.API.Modules.Auth.User;

[TestClass]
public class LoginEndpointTests
{
    private LabContext _context = null!;
    private IJWTHelper _jwtHelper = null!;

    [TestInitialize]
    public void Setup()
    {
        ✅ 建立 In-Memory 資料庫
        var options = new DbContextOptionsBuilder<LabContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new LabContext(options);

        ✅ 使用 NSubstitute 建立 Mock JWT Helper
        _jwtHelper = Substitute.For<IJWTHelper>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    [TestMethod]
    public async Task Handler_ValidCredentials_ReturnsOk()
    {
        ✅ 3A測試 Arrange Act Assert
        // Arrange
        var userId = "admin";
        var password = "p@ssw0rd";
        var role = "Admin";
        var token = "mock-jwt-token";
        var expiresIn = 60;

        // 準備測試資料
        await DatabaseTestHelper.CreateTestUserAsync(_context, userId, password, role);

        // Mock JWT Helper 行為
        _jwtHelper
            .GenerateToken(userId, Arg.Any<string>(), Arg.Any<IList<string>>())
            .Returns(token);
        _jwtHelper.GetExpiresIn().Returns(expiresIn);

        var request = new LoginRequest { UserId = userId, Password = password };

        // Act
        var result = await LoginEndpoint.Handler(
            request,
            _context,
            _jwtHelper,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<LoginResponse>>));

        var okResult = (Ok<APIResponse<LoginResponse>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("登入成功", okResult.Value.Message);
        Assert.AreEqual(token, okResult.Value.Data!.Token);
        Assert.AreEqual(expiresIn, okResult.Value.Data!.ExpiresIn);

        ✅ 驗證 JWT Helper 被正確呼叫
        _jwtHelper
            .Received(1)
            .GenerateToken(
                userId,
                Arg.Any<string>(),
                Arg.Is<IList<string>>(roles => roles.Contains(role))
            );
        _jwtHelper.Received(1).GetExpiresIn();
    }
}

```

## 常用工具 MSTest 單元測試

主要測試引用專案的 Adapter、Helper、Service、Repository 等類別的方法

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
