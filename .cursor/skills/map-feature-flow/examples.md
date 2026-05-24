# 完整範例

> 下面是「登入」功能產出的 `docs/features/login.md` 完整內容，作為**目標長相**參考。
>
> 範例使用 Vue 3 + .NET Minimal API，但模板適用任何前後端框架。
>
> **連結格式**：所有檔案連結用 `file:///{repo-root}/{相對路徑}#L{行號}`（沒有行號就省 `#L`），`{repo-root}` 由 `git rev-parse --show-toplevel` 取得。範例中用 `{repo-root}` 表示，實際輸出時替換為真路徑（如 `D:/程式/Lab.Todo`）。
>
> 三種模式（功能名 / 頁面 / endpoint）的輸出**章節結構都一樣**，差別只在切入點。

---

```markdown
# 登入

## 快速導覽

使用者在 `/login` 輸入帳密 → 打 `POST /api/user/login` → 後端查 DB 驗證 → 成功簽 JWT 回前端 → 前端存 cookie + 導頁。失敗回 422，由 axios 統一 toast。**不寫任何 DB**。

## 入口

- 路由：`/login` → [Login.vue](file:///{repo-root}/src/todo-web/src/views/Login.vue)
- 怎麼被打開:
    - 直接輸入 URL
    - 未登入訪問需認證頁 → [router guard](file:///{repo-root}/src/todo-web/src/router/index.js#L60) 自動導來
    - 其他 API 回 401 → [axios interceptor](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L74) 清 cookie 自動導回

## 使用者操作

- **表單送出 Sign In** → `onFormSubmit` ([Login.vue:30](file:///{repo-root}/src/todo-web/src/views/Login.vue#L30))

> Remember me、Forgot password? 目前**無實作**（純 UI）

## 呼叫的 API

| Method + Path              | 契約                                                       |
| -------------------------- | ---------------------------------------------------------- |
| **`POST /api/user/login`** | [docs/api/login.md](file:///{repo-root}/docs/api/login.md) |

- 前端：[auth-api.js:7](file:///{repo-root}/src/todo-web/src/services/auth-api.js#L7)
- 後端: [LoginEndpoint.cs:5](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L5)

## 做了什麼判斷

**前端送出前** — [Login.vue](file:///{repo-root}/src/todo-web/src/views/Login.vue)

- Zod 驗證 `userId`、`password` 為空 → 顯示欄位錯誤，**不送 API**
- `valid === false` → 直接 return

**後端** — [LoginEndpoint.cs](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs)

- Body 欄位驗證失敗 → `400` + validationErrors
- 帳號或密碼錯 → `422`「帳號或密碼不正確」
- 通過 → 簽 JWT、回 `200` + token + expiresIn

**路由守衛** — [router/index.js](file:///{repo-root}/src/todo-web/src/router/index.js)

- 已登入卻訪問 `/login` → 自動導去 `redirect` 或 `/`
- 未登入訪問需認證頁 → 導向 `/login`，記住原路徑

## 成功後做什麼

- **前端**：`code === 2000` 且有 `data.token` → 寫入 cookie（[useAuth.js:16](file:///{repo-root}/src/todo-web/src/composables/useAuth.js#L16)）→ 成功 toast → 導頁
- **後端**：簽 JWT（[JWTHelper.cs:5](file:///{repo-root}/src/TodoAPI/Infrastructures/Security/JWT/JWTHelper.cs#L5)）；**不寫 DB**

## 順帶會發生的事

- **Log**：全域 middleware 記 request/response body（[RequestResponseLoggingMiddleware.cs:12](file:///{repo-root}/src/TodoAPI/Infrastructures/Logging/RequestResponseLoggingMiddleware.cs#L12)）
- **Toast**：API 錯誤統一由 axios interceptor 顯示（[axios-instance.js:52](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L52)）

## 相關

- API 契約：[docs/api/login.md](file:///{repo-root}/docs/api/login.md)
- 主要 Entity：[User.cs](file:///{repo-root}/src/TodoAPI/Infrastructures/Data/Entities/User.cs)
```

---

## 三模式的切入差異

| 模式     | 切入                               | 速覽用什麼角度寫              |
| -------- | ---------------------------------- | ----------------------------- |
| 功能名   | grep 中英文找路由 / 頁面 / handler | 使用者要做什麼                |
| 頁面     | 直接讀頁面元件檔                   | 這個畫面在幹嘛                |
| endpoint | 讀 endpoint → URL 反查前端呼叫者   | 這個 API 在誰那邊被用、整鏈路 |

差別只在**速覽的敘述角度**與**「入口」段是否要列多個呼叫者**。
