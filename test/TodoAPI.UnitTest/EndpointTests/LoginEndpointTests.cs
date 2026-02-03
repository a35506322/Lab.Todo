using TodoAPI.Modules.Auth.User.Login;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class LoginEndpointTests
{
    private LabContext _context = null!;
    private IJWTHelper _jwtHelper = null!;

    [TestInitialize]
    public void Setup()
    {
        // 建立 In-Memory 資料庫
        var options = new DbContextOptionsBuilder<LabContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new LabContext(options);

        // 建立 Mock JWT Helper
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

        // 驗證 JWT Helper 被正確呼叫
        _jwtHelper
            .Received(1)
            .GenerateToken(
                userId,
                Arg.Any<string>(),
                Arg.Is<IList<string>>(roles => roles.Contains(role))
            );
        _jwtHelper.Received(1).GetExpiresIn();
    }

    [TestMethod]
    public async Task Handler_InvalidCredentials_ReturnsBusinessLogicError()
    {
        // Arrange
        var request = new LoginRequest { UserId = "invalid", Password = "wrong" };

        // Act
        var result = await LoginEndpoint.Handler(
            request,
            _context,
            _jwtHelper,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnprocessableEntity<APIResponse<LoginResponse>>));

        var errorResult = (UnprocessableEntity<APIResponse<LoginResponse>>)result;
        Assert.IsNotNull(errorResult.Value);
        Assert.AreEqual(Code.商業邏輯錯誤, errorResult.Value.Code);
        Assert.AreEqual("帳號或密碼不正確", errorResult.Value.Message);

        // 驗證 JWT Helper 沒有被呼叫
        _jwtHelper
            .DidNotReceive()
            .GenerateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IList<string>>());
    }

    [TestMethod]
    public async Task Handler_WrongPassword_ReturnsBusinessLogicError()
    {
        // Arrange
        var userId = "admin";
        var correctPassword = "p@ssw0rd";
        var wrongPassword = "wrong";

        await DatabaseTestHelper.CreateTestUserAsync(_context, userId, correctPassword, "Admin");

        var request = new LoginRequest { UserId = userId, Password = wrongPassword };

        // Act
        var result = await LoginEndpoint.Handler(
            request,
            _context,
            _jwtHelper,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnprocessableEntity<APIResponse<LoginResponse>>));

        var errorResult = (UnprocessableEntity<APIResponse<LoginResponse>>)result;
        Assert.AreEqual(Code.商業邏輯錯誤, errorResult.Value.Code);
    }
}
