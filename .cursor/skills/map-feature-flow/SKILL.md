---
name: map-feature-flow
description: 把一個功能/頁面/API endpoint 翻譯成「考前重點」式的檔案地圖：速覽段一眼看懂、條列為主、每條掛檔案位置但不解釋程式碼。使用時機：使用者要快速了解某個功能/頁面在做什麼、呼叫哪些 API、做了什麼判斷，例如「登入流程怎麼跑」「XxxPage 在幹嘛」「XxxEndpoint 是被誰用的」。支援任何前後端框架（React / Vue / Svelte / Angular + Express / FastAPI / Spring / .NET / Rails / Gin 等）。
---

# 功能流程地圖（Feature Flow Map）

## 使用時機

把一個功能/頁面/endpoint 翻譯成「**考前重點整理**」風格的檔案地圖：

- **速覽段**一眼看懂在幹嘛
- **條列為主**，表格只用在多 API 對照
- 每點附檔案位置但**不解釋程式碼**

例如：

- 「登入流程怎麼跑」
- 「`XxxPage` 在做什麼」
- 「`XxxEndpoint` 是被誰呼叫的」

## 輸入與輸出

| 觸發模式 | 輸入範例                                           | 起點                                              |
| -------- | -------------------------------------------------- | ------------------------------------------------- |
| 功能名   | `登入`、`新增 todo`                                | grep 中英文關鍵字 → 路由 / page / handler / store |
| 頁面     | `LoginPage.vue` / `LoginPage.tsx` / `login.svelte` | 直接讀頁面元件檔                                  |
| Endpoint | `LoginEndpoint.cs` / `login_controller.py`         | 直接讀 endpoint 檔，反向找前端使用者              |

| 項目     | 說明                                      |
| -------- | ----------------------------------------- |
| 輸出     | `docs/features/[name].md`（不存在則建立） |
| 檔名規則 | 功能英文名 kebab-case，例：`login.md`     |

## 三種模式的定位

三模式只是**切入點**不同，產出**都是「以起點為中心的功能脈絡」**（不是「只看起點本身」）：

| 模式     | 切入點          | 重點                                  |
| -------- | --------------- | ------------------------------------- |
| 功能名   | 使用者故事      | 從使用者要做的事展開整條鏈路          |
| 頁面     | 這個畫面在幹嘛  | 從頁面展開所有它觸發的 API 與後端邏輯 |
| endpoint | 這個 API 被誰用 | 從 endpoint 反查呼叫者，再展開全鏈路  |

**展開的章節都一樣**（速覽 / 入口 / 操作 / API / 判斷 / 成功後 / 副作用）。差別只在「**速覽用哪個角度敘述**」與「**入口段是否要列多個呼叫者**」。

> `map-feature-flow` 是使用脈絡（被誰、何時、為什麼呼叫）。endpoint 模式產出**不是規格文件**。

## 執行流程

### Step 0：抓 repo root

跑 `git rev-parse --show-toplevel` 取得 repo 絕對路徑（如 `D:/程式/Lab.Todo`），記下來。**所有連結 URL 都用 `file:///{repo-root}/...#L{line}` 為前綴**（理由見硬規則 #6）。

### Step 1：確認觸發模式

模糊（如「todo」）→ 向使用者確認**具體名稱**與**期望起點**。

### Step 2：定位起點

- **功能名**：grep 中文 → grep 英文 → 看 router 設定檔
- **頁面**：直接讀頁面元件檔
- **endpoint**：讀檔 → **用 URL 字串反向 grep 前端 HTTP client 呼叫處**

### Step 3：雙向追蹤（深度停在資料存取層）

**前端**（依框架類比）：

| 層           | 來源                                                             |
| ------------ | ---------------------------------------------------------------- |
| View         | template / JSX / 模板字串                                        |
| 事件 handler | `@click` / `onClick` / `bind:click` / `(click)` 等               |
| 狀態管理     | store / context / signal / atom（Pinia / Redux / Zustand 等）    |
| HTTP client  | service / hook / composable（axios / fetch / SWR / TanStack 等） |

**後端**（依框架類比）：

| 層         | 來源                                                            |
| ---------- | --------------------------------------------------------------- |
| Endpoint   | route attribute / decorator / router 方法 + 上層 group / prefix |
| Handler    | endpoint 檔內 / controller method / handler function            |
| 業務邏輯層 | service / use case / domain service / interactor                |
| 資料存取層 | repository / DAO / ORM context / query builder                  |

**停止條件**：到資料存取層即停，**DB schema 不展開**。

### Step 4：副作用掃描

只記錄**真正存在**的（沒有的不要列「（無）」）：

| 類型                | 來源                                                  |
| ------------------- | ----------------------------------------------------- |
| Log                 | logger 介面呼叫（`logger.*` / `log.*` / `console.*`） |
| Email / 通知        | mailer / notifier / push / SMS                        |
| 外部 API            | HTTP client / 第三方 SDK                              |
| Event / Message     | event bus / MQ / pub-sub / domain event               |
| 快取                | cache client（in-memory / distributed）               |
| 排程 / 背景工作     | scheduler / queue / hosted service                    |
| 檔案系統 / 物件儲存 | 本機檔 / S3 / Blob / GCS                              |
| 認證 / 授權變更     | session / token 簽發 / cookie 寫入                    |

### Step 5：套用模板輸出

見下方「輸出模板」。

### Step 6：寫入檔案

寫到 `docs/features/[name].md`。

---

## 硬規則（最重要）

1. **速覽段必寫且放最前面**
    - 3-5 行白話：「使用者從哪進來 → 打哪些 API → 後端做什麼 → 動了什麼狀態」
    - **最後一句**寫關鍵狀態，例：「不寫 DB」「只動 X 表」「會發 webhook」
2. **條列為主，表格只用在多 API 對照**
    - 「呼叫的 API」用表格；其他章節**一律條列**
3. **「沒做的事」用 `>` 引用一行帶過**
    - ✅ `> Remember me、Forgot password? 目前無實作（純 UI）`
    - ❌ 表格列「（無 handler）」「（無 handler）」
4. **「（無）」的副作用類型直接砍**
    - 沒有 Email 副作用 → 整列刪除，不要寫「Email：（無）」
5. **章節按需出現**
    - 沒內容的章節（如「副作用」全空）整個砍掉，不留空殼
6. **檔案位置用 markdown 連結（絕對路徑 + `#L` 行號）**
    - 格式：`[檔名:行號](file:///{repo-root}/{相對路徑}#L{行號})`
    - 例：`[Login.vue:30](file:///D:/程式/Lab.Todo/src/todo-web/src/views/Login.vue#L30)`
    - 沒有行號就省 `#L`：`[Login.vue](file:///{repo-root}/src/todo-web/src/views/Login.vue)`
    - `{repo-root}` 由 Step 0 取得
    - **為什麼**：Cursor markdown preview 只認 `file://` + `#L行號`，`src/foo:42` 這種終端機格式點不開
    - **代價**：路徑寫死，跨機器需重跑 skill 重新產出 — 可接受，因為此 doc 本來就是「給當下環境快速理解脈絡」用
    - 條列下置或段落末，**不要塞進句子中段把句子撐爛**
7. **寫「做什麼」不寫「怎麼做」**
    - ✅「驗證帳密」「寫入 todo 表」
    - ❌「用 bcrypt 比對 hash」「呼叫 ORM `.Add()` 後 `.SaveChanges()`」
8. **條件只寫條件本身**
    - ✅「若 token 過期 → 401」
    - ❌「middleware try decode JWT，catch `XxxExpiredException` ...」
9. **判斷條件能精確就精確**
    - ✅「`valid === false` → 直接 return」「`code === 2000` 且有 `data.token` → 寫 cookie」
    - ❌「驗證失敗不送」「成功就導頁」
    - 條件描述要能讓讀者直接對到程式碼比對的值
10. **不複製 API 契約 / 不展開 DB schema**
    - 連結指過去即可
11. **不確定就明說**
    - 「我推測這條由 X 觸發，請確認」

---

## 輸出模板

> 模板**框架無關**，範例為示意，實際路徑與副檔名依專案替換。
>
> **章節按需出現**：沒內容的章節直接砍，不留空殼。
>
> **連結格式**：下方模板 `(路徑)` 為簡寫示意；實際輸出時所有連結 URL 必須是 `file:///{repo-root}/{相對路徑}#L{行號}` 格式（見硬規則 #6 與 examples.md）。

```markdown
# [功能中文名稱]

## 速覽

3-5 行白話。使用者從 `/路徑` 進來 → 打 `POST /api/xxx` → 後端做 X → 成功後動 Y / 失敗 XXX。**[關鍵狀態：不寫 DB / 只動 X 表 / 會發 webhook 等]**

## 入口

- 路由：`/路徑` → [頁面元件](路徑)
- 怎麼被打開：
    - 直接輸入 URL
    - 從 X 跳轉
    - 條件觸發（如 token 過期自動導）

## 使用者操作

- **操作描述** → `handlerName` ([檔案:行號](路徑:行號))

> 若有純 UI 無實作元素：「X、Y 目前無實作（純 UI）」

## 呼叫的 API

| Method + Path       | 契約                    |
| ------------------- | ----------------------- |
| **`POST /api/xxx`** | [docs/api/xxx.md](路徑) |

- 前端：[檔案:行號](路徑:行號)
- 後端：[檔案:行號](路徑:行號)

> 多個 API 時表格多列，每列下方各附前後端定位

## 做了什麼判斷

**前端送出前** — [頁面檔案](路徑)

- 條件 → 動作

**後端** — [endpoint 檔案](路徑)

- 條件 A → `4XX` + 訊息
- 條件 B → `4XX` + 訊息
- 通過 → `200` + 回什麼

**其他守衛**（如路由守衛、權限攔截、middleware）— [檔案](路徑)

- 條件 → 動作

## 成功後做什麼

- **前端**：做了什麼（[檔案](路徑)）
- **後端**：做了什麼；**是否寫 DB**

## 副作用

只列實際存在的：

- **Log**：說明（[檔案](路徑)）
- **快取**：說明（[檔案](路徑)）

> 全空則整個章節刪除

## 相關

- API 契約：[docs/api/...](路徑)
- 主要 Entity：[檔案](路徑)
```

---

## 注意事項

- **權限/授權檢查**通常在上層 group / middleware / decorator，**寫在「做了什麼判斷」段最上方**
- **跨層追蹤要實際開檔**：不要憑直覺寫；不確定就 grep + 讀
- **一個功能跨多個 endpoint**：API 表格多列、判斷段依執行順序排列
- **頁面模式**：若有複雜前端條件渲染（按鈕禁用、欄位切顯），追加一節「前端條件渲染」用條列
- **endpoint 模式**：「入口」段可能有多個前端入口呼叫同一 endpoint，逐一列出
- **完整範例見** [examples.md](examples.md)
