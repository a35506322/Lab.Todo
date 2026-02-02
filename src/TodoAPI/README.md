# Lab.TodoAPI

## Framework

-   [x] ASP.NET Core 10

## Features

-   [x] Minimal API + 垂直切割
-   [x] <del>Swagger UI</del> -> Scalar UI (含 Request/Response 範例)
-   [x] JWT 驗證
-   [x] Model 驗證 + 統一回傳驗證錯誤訊息
-   [x] 資料存取工具 (Dapper+ Entity Framework Core)
-   [ ] 測試專案 (MSTest)
-   [x] Logging (Serilog)
-   [x] Exception Handling (Middleware)
-   [ ] HttpClient 最佳示範
-   [ ] CORS 設定
-   [x] RBAC 角色權限管理 (簡易)

## 建議

以下功能因每個系統的需求不同，如需要在自行實作

1. RBAC 角色權限管理 (可因應需求而更改)
2. 使用 FusionCache 做快取
3. 驗證錯誤訊息本地化
    - Minimal API 需透過 註冊 `CustomProblemDetailsService` 取代預設 `IProblemDetailsService`，在 `WriteAsync` 內對 `HttpValidationProblemDetails` 呼叫 `ValidationErrorLocalizer.Localize()` 轉成中文
    - Controller API 可用 [參考文章](https://cloudywing.github.io/backend/%E5%A6%82%E4%BD%95%E5%AE%A2%E8%A3%BD%E5%8C%96%20ASP.NET%20Core%20%E7%9A%84%20Model%20Validation%20%E9%A0%90%E8%A8%AD%E9%8C%AF%E8%AA%A4%E8%A8%8A%E6%81%AF) 比較正規作法。
    - 日後如果有更好做法可以取代
4. 如之後有共用專案如排程等，可以考慮將 Common 資料夾獨立成類別庫
5. Log 收集器強烈推薦使用 Seq，如果要使用在 Serlog 設定即可
