---
name: create-api-spec
description: 從單一 API endpoint 程式碼產出規格文件（Markdown）。使用時機：使用者給你 endpoint 程式碼路徑要求產出 API spec，例如「把 XxxEndpoint.cs 寫成 spec」「幫這個 controller 產出 API 文件」「產 spec.md」。支援任何後端框架（.NET / Node / Python / Java 等），流程框架無關。
---

# 從 API endpoint 產出規格文件

## 使用時機

使用者給你**單一 API endpoint 程式碼檔案路徑**，希望產出對應規格文件（Markdown）。

例如：

- 「把 `src/.../GetTodoByIdEndpoint.cs` 寫成 spec」
- 「幫這個 controller 產出 API 文件」
- 「產 `login` 的 spec.md」

## 輸入與輸出

| 項目     | 說明                                                                                        |
| -------- | ------------------------------------------------------------------------------------------- |
| 輸入     | 一個 endpoint 程式碼檔案路徑（一次一個）                                                    |
| 輸出     | `docs/api/[name].md`（若資料夾不存在則建立）                                                |
| 檔名規則 | endpoint 名稱轉 kebab-case，例如：`GetTodoById` → `get-todo-by-id.md`、`Login` → `login.md` |

## 執行流程（框架無關）

### Step 1：識別框架

從副檔名與內容推斷：

- `.cs` → .NET（Minimal API / Controller）
- `.ts` / `.js` → Node（Express / Nest / Fastify / Hono）
- `.py` → Python（FastAPI / Django / Flask）
- `.java` / `.kt` → Spring Boot
- `.go` → Gin / Echo / Fiber
- 其他依檔案內容判斷

### Step 2：蒐集 8 項必要資訊

每個框架都要找出以下資訊，**找法視框架而定，由你類比推導**：

| #   | 項目                  | 來源（依框架類比）                                                                                                                           |
| --- | --------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | HTTP method           | route attribute / decorator / router 方法                                                                                                    |
| 2   | 完整 URL              | 往上層 router / group / prefix 找，**組裝完整路徑**（不只看當前檔案）                                                                        |
| 3   | 認證需求              | auth attribute / decorator / middleware / 上層 group 的 require auth                                                                         |
| 4   | Path params           | URL pattern（`{id}`、`:id` 等）+ 路由限制（如型別）                                                                                          |
| 5   | Query / Header / Body | handler 簽章參數 或 DTO / Schema 定義                                                                                                        |
| 6   | 成功回應 (2xx)        | 回傳型別 / response model / serialization target                                                                                             |
| 7   | 錯誤回應 (4xx / 5xx)  | 綜合三處：<br>① handler 內 `throw` / `return error`<br>② attribute / decorator 標註的 response type<br>③ 全域 exception handler / middleware |
| 8   | 範例 JSON             | example provider / docstring / sample 註解；無則依 DTO 結構自行推合理範例                                                                    |

### Step 3：確認通用錯誤格式（response envelope）

產文件前**先掃專案有無統一回應結構**：

1. 找 global exception handler / error middleware → 確認所有錯誤是否被包成統一結構
2. 找 response wrapper（如 `APIResponse<T>`、`ResponseEnvelope`、`Result<T>`）→ 成功/失敗結構是否一致
3. **有則所有錯誤回應套用該格式**
4. 若某 endpoint 自訂 return 不同結構 → 在文件中註明「此 endpoint 自訂回應格式」

### Step 4：套用模板輸出

見下方「輸出模板」。

### Step 5：寫入檔案

寫到 `docs/api/[name].md`。若 `docs/api/` 不存在則建立。

---

## 輸出模板

````markdown
# [Endpoint 中文描述名稱]

## 基本資訊

| 項目   | 內容                                  |
| ------ | ------------------------------------- |
| URL    | `[完整 URL，含所有 prefix]`           |
| Method | [HTTP method]                         |
| 描述   | [從程式碼註解 / summary attribute 取] |
| 認證   | [需要 JWT / 不需要 / 需要特定角色]    |

## Request

### Path Parameters

| 欄位 | 型別 | 必填 | 說明 | 範例 |
| ---- | ---- | ---- | ---- | ---- |
| id   | int  | Y    | xxx  | 1    |

> 無則寫「（無）」

### Query Parameters

| 欄位 | 型別 | 必填 | 說明 | 範例 |
| ---- | ---- | ---- | ---- | ---- |

> 欄位名稱用**程式碼屬性原名**（.NET 是 `PascalCase`），不轉序列化命名
> 多條件行為要在表格下方註明，如「多條件為 AND」「空值不套篩選」「排序預設 desc」
> 無則寫「（無）」

### Headers

| 欄位 | 必填 | 說明 |
| ---- | ---- | ---- |

> 依實際需要列出：
>
> - 需要認證 → `Authorization: Bearer {token}`
> - POST / PUT / PATCH 帶 JSON body → `Content-Type: application/json`
> - 無則寫「（無）」

### Body

| 欄位 | 型別 | 必填 | 說明 | 範例 |
| ---- | ---- | ---- | ---- | ---- |

> 欄位名稱用 **JSON 序列化後的命名**（.NET 預設 `camelCase`）
> 巢狀欄位用點記法：`user.name`、`items[].title`
> 無則寫「（無）」

### Example

```json
{ ... }
```

> 無 body 則寫「（無）」
> **URL 範例 block** — 有 path param 或多種 query 組合時，補一個 URL 範例展示實際呼叫格式：
>
> GET / DELETE 多 query 組合：
>
> ```
> 查詢範例：
> GET /api/xxx
> GET /api/xxx?TodoTitle=測試
> GET /api/xxx?TodoTitle=測試&IsComplete=N
> ```
>
> PUT / POST / PATCH / DELETE 帶 path param（放在 body example 區塊下方）：
>
> ```
> PUT /api/todo/updateTodoById/1
> ```

## Response

### 成功

#### 200 OK

**欄位說明**

| 欄位 | 型別 | 說明 | 範例 |
| ---- | ---- | ---- | ---- |

**Example**

```json
{ ... }
```

#### 201 Created

> 多個成功狀態時新增區塊，同上格式

### 錯誤

| HTTP Code | 觸發條件                                     | Example   |
| --------- | -------------------------------------------- | --------- |
| 401       | xxx                                          | `{ ... }` |
| 404       | xxx                                          | `{ ... }` |
| 500       | 未捕捉的例外（由全域 ExceptionHandler 處理） | `{ ... }` |

> **反向澄清主動加**（用 `>` 引用區塊在錯誤表格下方註明，覆蓋以下情境）：
>
> - 若 endpoint 不回某些常見錯誤碼，例：「無 422」「無 401／403」
> - 查無資料的特殊行為，例：「查無資料仍回 200，data 為 `[]`」
> - 踩雷提醒，例：「帳密錯誤走 422 而非 401」「HTTP 200 也可能搭配非 2000 的 envelope code」
````

---

## 完整範例

完整的輸入 → 輸出對照、以及推導模式說明，請讀 [examples.md](examples.md)。

範例為 .NET Minimal API，但**其他框架同理**：找路由、找回應結構、找錯誤處理，套同一份模板。

---

## 注意事項

- **巢狀欄位用點記法**：`user.name`、`items[].title`
- **enum 序列化要看實際**：可能是名稱字串、可能是數字，依專案 JSON 設定判斷，不要憑記憶猜
- **欄位命名規則因情境而異**：
    - **Body 欄位** → JSON 序列化後的名稱（.NET 預設 `camelCase`、Python 預設原樣）
    - **Query / Path 欄位** → 程式碼屬性原名（.NET 是 `PascalCase`）；多數框架 model binding 是 case-insensitive，但寫程式碼原名與程式碼對齊更直觀
    - **`validationErrors` 的 key** → 框架實際輸出格式（.NET 通常是 C# 屬性原名 `PascalCase`）
- **統一錯誤格式優先**：先確認專案 envelope，再以此格式產 example；該 endpoint 若自訂回應結構要特別標出
- **example 來源優先序**：① 程式碼內 example provider > ② docstring / 註解內樣本 > ③ 依 DTO 結構自行推合理範例
- **錯誤訊息用實際字串**：401 / 403 的 message 要去專案的 SecurityConfig / AuthMiddleware 查實際註冊的字串，不要直接寫 Code enum 名稱
- **反向澄清主動加**：endpoint 不回的錯誤碼、特殊資料行為、踩雷提醒，都用 `>` 引用區塊註明（詳見模板「錯誤」段落）
- **表格欄位 padding 對齊**：寫出後手動對齊各欄寬度，提升可讀性
- **不確定的細節要說出來**：例如「我推測 enum 序列化為數字，請確認」，不要硬寫

---

## 自我擴充原則

每次跑完 skill 產出 spec 後，**主動檢查**是否有「目前 skill 規則未明確涵蓋」的判斷或處理。若有，**立即執行以下三步**：

1. **擴充 skill 規則**
    - 能歸納進現有條目 → 加成子項目或範例庫
    - 完全是新類型 → 開新條目
    - 寫法：判斷原則 + 掃描步驟 + 範例，避免「為單一案例新增整條規則」

### 何時觸發

- **對照標準產出時**：使用者既有的 spec 跟我產出的有差異 → 校準
- **產出全新 spec 時**：自己審視「有沒有寫進 spec 但 skill 沒明確要求」的判斷 → 把這個判斷抽象成規則
- **使用者反饋時**：使用者指出產出缺漏 → 把該類缺漏的判斷原則寫進 skill

### 收斂判準

- 若連續 3 個 endpoint 校準都沒新規則 → skill 已穩定，停止主動擴充
- 若同一規則重複出現在多個校準紀錄 → 把規則升級為「掃描步驟」必跑項，避免遺漏
