namespace TodoAPI.Infrastructures.JWT;

public class JWTHelper(IConfiguration configuration) : IJWTHelper
{
    public string GenerateToken(string userName, string? userId = null, IList<string>? roles = null)
    {
        var issuer = configuration.GetValue<string>("JwtSettings:Issuer");
        var signKey = configuration.GetValue<string>("JwtSettings:SignKey");
        var audience = configuration.GetValue<string>("JwtSettings:Audience");
        var expireMinutes = configuration.GetValue<int>("JwtSettings:ExpireMinutes");

        if (string.IsNullOrEmpty(signKey) || signKey.Length < 32)
            throw new InvalidOperationException("JwtSettings:SignKey 至少 32 字元");

        var claims = new List<Claim>
        {
            // Subject：token 代表的主體，通常是使用者唯一識別（例如 userId）
            new Claim(JwtRegisteredClaimNames.Sub, userId ?? userName),
            // 使用者名稱（或顯示名稱）。ASP.NET Core 預設會把某個 claim 對應到 User.Identity.Name，若驗證端用 NameClaimType = "unique_name"，這裡的值就會變成登入後 User.Identity?.Name。
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            // JWT ID：這張 token 的唯一識別碼（通常用 GUID）。可用來做單次使用、黑名單、或除錯追蹤，RFC 7519 建議每張 token 都有唯一的 jti。
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // 發行時間：token 發行的時間（Unix 時間戳）。可用來驗證 token 是否過期，RFC 7519 建議每張 token 都有唯一的 iat。
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
        };

        if (roles?.Count > 0)
        {
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var userClaimsIdentity = new ClaimsIdentity(claims);

        // Create a SymmetricSecurityKey for JWT Token signatures
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

        // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
        // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256Signature
        );

        // Create SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            //NotBefore = DateTime.Now, // Default is DateTime.Now
            //IssuedAt = DateTime.Now, // Default is DateTime.Now
            Subject = userClaimsIdentity,
            Expires = DateTime.Now.AddMinutes(expireMinutes),
            SigningCredentials = signingCredentials,
        };

        // Generate a JWT, than get the serialized Token result (string)
        var tokenHandler = new JsonWebTokenHandler();
        var serializeToken = tokenHandler.CreateToken(tokenDescriptor);

        return serializeToken;
    }

    public int GetExpiresIn() => configuration.GetValue<int>("JwtSettings:ExpireMinutes");
}
