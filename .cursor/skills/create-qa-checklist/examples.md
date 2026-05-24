# 完整範例

> 下面是從 `docs/features/login.md` 產出的 `docs/qa/login.md`（full 模式）完整內容，作為**目標長相**參考。
>
> 範例為 Vue 3 + .NET，但模板與規則框架無關。
>
> **連結格式**：所有檔案連結用 `file:///{repo-root}/{相對路徑}#L{行號}`，`{repo-root}` 由 `git rev-parse --show-toplevel` 取得。範例中用 `{repo-root}` 表示，實際輸出時替換為真路徑（如 `D:/程式/Lab.Todo`）。
>
> **引用縮排**：todo 下方 `>` 引用一律 4 個 space 起頭。

---

````markdown
# 登入 - 驗收清單

> **模式**：全鏈路（full）
> **來源 flow**：[docs/features/login.md](file:///{repo-root}/docs/features/login.md)
> **產出時間**：2026-05-24
> **圖例**：`[x]` 程式碼已實作（請手動驗證行為正確）；`[ ]` 未實作 / 純 UI / 待補測

## 正常流程

- [x] 直接輸入 `/login` URL → 顯示登入表單（帳號、密碼、Sign In 按鈕）
    > **證據**：肉眼觀察頁面元素
    > **實作**：[router/index.js:34](file:///{repo-root}/src/todo-web/src/router/index.js#L34)、[Login.vue:116](file:///{repo-root}/src/todo-web/src/views/Login.vue#L116)
- [x] 輸入正確帳密、按 Sign In → toast 顯示「登入成功」，URL 導向 `/`
    > **證據**：右上角 toast、URL bar
    > **實作**：[Login.vue:41](file:///{repo-root}/src/todo-web/src/views/Login.vue#L41)、[Login.vue:46](file:///{repo-root}/src/todo-web/src/views/Login.vue#L46)
- [x] 登入成功後 DevTools → Application → Cookies 有 `token`，`Max-Age` 約等於 API 回的 `expiresIn × 60` 秒
    > **證據**：DevTools Application 面板
    > **實作**：[useAuth.js:16](file:///{repo-root}/src/todo-web/src/composables/useAuth.js#L16)
- [x] Network 看到 `POST /user/login` 回 `200`，response body 含 `token` + `expiresIn`
    > **證據**：DevTools Network → Response
    > **實作**：[LoginEndpoint.cs:20](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L20)、[auth-api.js:7](file:///{repo-root}/src/todo-web/src/services/auth-api.js#L7)

## 表單檢查

- [x] 帳號空白、按 Sign In → 欄位下顯示「帳號為必填」，Network 無 `POST /user/login`
    > **證據**：欄位下方 `<Message>`、DevTools Network
    > **實作**：[Login.vue:25](file:///{repo-root}/src/todo-web/src/views/Login.vue#L25)、[Login.vue:31](file:///{repo-root}/src/todo-web/src/views/Login.vue#L31)
- [x] 密碼空白、按 Sign In → 欄位下顯示「密碼為必填」，Network 無 `POST /user/login`
    > **證據**：欄位下方 `<Message>`、DevTools Network
    > **實作**：[Login.vue:26](file:///{repo-root}/src/todo-web/src/views/Login.vue#L26)、[Login.vue:31](file:///{repo-root}/src/todo-web/src/views/Login.vue#L31)
- [x] 帳號 / 密碼前後純空白（如 `"   "`） → 視為空，跳必填訊息
    > **證據**：Zod `trim().min(1)` 行為
    > **實作**：[Login.vue:25](file:///{repo-root}/src/todo-web/src/views/Login.vue#L25)

## 錯誤情境

- [x] 錯誤帳號或密碼 → toast summary「商業邏輯錯誤」、detail「帳號或密碼不正確」（detail 被後端 `message` 覆蓋），Cookies 無 `token`，停留在 `/login`
    > **證據**：toast 文字、Network 回 `422`、DevTools Application Cookies、URL bar
    > **實作**：[LoginEndpoint.cs:36](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L36)、[axios-instance.js:52](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L52)
- [x] 後端服務關閉送出 → toast summary「網路連線異常」、detail「無法連線至伺服器，請確認網路狀態或稍後再試」
    > **證據**：toast summary + detail 文字
    > **實作**：[axios-instance.js:65](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L65)
- [x] 請求逾時（DevTools 模擬 slow 3G 或拔網路） → toast summary「請求逾時」、detail「伺服器回應時間過長，請稍後再試」
    > **證據**：toast summary + detail 文字
    > **實作**：[axios-instance.js:58](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L58)

## 進出頁面

- [x] 未登入直接訪問 `/todo` → toast summary「登入失敗」、detail「帳號或密碼不正確，或者登入已過期」，URL 被導到 `/login?redirect=/todo`
    > **證據**：toast 文字、URL bar
    > **實作**：[router/index.js:60](file:///{repo-root}/src/todo-web/src/router/index.js#L60)
- [x] 從 `/login?redirect=/todo` 登入成功 → URL 導回 `/todo`
    > **證據**：URL bar
    > **實作**：[Login.vue:46](file:///{repo-root}/src/todo-web/src/views/Login.vue#L46)
- [x] 已登入再訪問 `/login` → 直接被導到 `redirect` 或 `/`
    > **證據**：URL bar
    > **實作**：[router/index.js:73](file:///{repo-root}/src/todo-web/src/router/index.js#L73)
- [x] 其他 API 回 `401` → Cookies 的 `token` 被清空，URL 跳到 `/login`
    > **證據**：DevTools Application Cookies、URL bar
    > **實作**：[axios-instance.js:74](file:///{repo-root}/src/todo-web/src/services/axios-instance.js#L74)

## 順帶會發生的事

- [x] 登入成功後 JWT decode 後 payload 含 `UserId` + `Role`
    > **證據**：[jwt.io](https://jwt.io) 貼 token decode
    > **實作**：[LoginEndpoint.cs:39](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L39)
- [x] 登入流程**不寫任何 DB**（不更新 last login 等欄位）
    > **證據**：登入前後查 `User` 表，所有欄位無變動
    > **實作**：[LoginEndpoint.cs:27](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L27)（`AsNoTracking`）

## 沒做的功能

- [ ] Remember me checkbox 勾選 → 應記住登入狀態（目前點擊無任何效果）
    > **證據**：N/A（目前無對應實作）
    > **實作**：未找到（flow 文件標註「純 UI」）— [Login.vue:172](file:///{repo-root}/src/todo-web/src/views/Login.vue#L172)
- [ ] Forgot password? 連結 → 應跳轉到忘記密碼頁（目前點擊無任何效果）
    > **證據**：N/A
    > **實作**：未找到（flow 文件標註「純 UI」）— [Login.vue:175](file:///{repo-root}/src/todo-web/src/views/Login.vue#L175)

## 額外建議測

> flow 文件未明確要求，但實務上建議手動驗證。

- [ ] 連續快速點擊 Sign In 5 次 → 只應發送 1 次 `POST /user/login`
    > **證據**：DevTools Network 請求數量
    > **理由**：避免 race condition / 後端重複認證壓力
- [ ] userId 輸入特殊字元（`'; DROP TABLE--`、emoji 等） → 應正常處理為一般字串、不應 500
    > **證據**：toast 為「帳號或密碼不正確」而非 500 訊息
    > **理由**：基本 SQL injection / 字元集相容驗證
- [ ] 密碼框 `type="password"` → 字元應以 `•` 顯示，不洩漏明碼
    > **證據**：肉眼觀察 / DevTools Elements
    > **理由**：基本安全 UX
- [ ] Tab 鍵順序：userId → password → Sign In
    > **證據**：鍵盤操作觀察
    > **理由**：鍵盤可達性
````

---

## 三模式輸出差異

同一份 flow 跑三種模式，砍掉 / 保留的章節不同：

| 章節                            | `full` | `api` | `web` |
| ------------------------------- | :----: | :---: | :---: |
| 正常流程                        |   ✓   |   ✓   |   ✓   |
| 表單檢查                        |   ✓   |   ✗   |   ✓   |
| 錯誤情境（前端 toast / 導頁）  |   ✓   |   ✗   |   ✓   |
| 錯誤情境（HTTP status）         |   ✓   |   ✓   |   ✗   |
| 進出頁面（前端 router）         |   ✓   |   ✗   |   ✓   |
| 進出頁面（後端 auth middleware）|   ✓   |   ✓   |   ✗   |
| 順帶會發生的事（cookie）        |   ✓   |   ✗   |   ✓   |
| 順帶會發生的事（DB / Log / JWT）|   ✓   |   ✓   |   ✗   |
| 沒做的功能                      |   ✓   |   ✗   |   ✓   |
| 額外建議測                      |   ✓   |   ✓   |   ✓   |

**檔名**：
- `full` → `docs/qa/login.md`
- `api`  → `docs/qa/login.api.md`
- `web`  → `docs/qa/login.web.md`

**示意**：`api` 模式的正常流程寫法

```markdown
- [x] Postman 送 `POST /user/login` 帶正確帳密 → 回 `200`，body 含 `token` + `expiresIn`
    > **證據**：Postman Response 面板
    > **實作**：[LoginEndpoint.cs:20](file:///{repo-root}/src/TodoAPI/Modules/Auth/User/Login/LoginEndpoint.cs#L20)
```

**示意**：`web` 模式的正常流程寫法

```markdown
- [x] 在 `/login` 輸入正確帳密、按 Sign In → toast 顯示「登入成功」，URL 導向 `/`
    > **證據**：右上角 toast、URL bar
    > **實作**：[Login.vue:41](file:///{repo-root}/src/todo-web/src/views/Login.vue#L41)
```

> 同一條正常流程、不同模式從不同視角寫斷言；不會兩邊都長一樣。
