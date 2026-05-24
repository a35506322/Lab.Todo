---
name: create-feature-todo
description: 從 logic flow 文件（`docs/features/*.md`，由 map-feature-flow 產出）抽出「功能 todo 清單」，輸出到 `docs/todo/<name>.md`。所有項目預設未勾選 `[ ]`，作為實作完成度快照；後續使用者會帶對標檔案請你比對打勾，但本 skill 只負責產初始清單。使用時機：使用者給你 logic flow 路徑要求產 todo，例如「幫 docs/features/todo.md 產 todo 清單」「把 login flow 變成 todo」。
---

# 從 logic flow 產出功能 todo 清單

## 使用時機

使用者給你**一份 logic flow 文件**（通常在 `docs/features/<name>.md`，由 `map-feature-flow` 產出），希望抽出「功能 todo 清單」做為實作完成度快照。

例如：

- 「幫 `docs/features/todo.md` 產 todo 清單」
- 「把這個 flow 變成 todo」
- 「列出 login feature 要做的功能」

## 輸入與輸出

| 項目 | 說明                                                                   |
| ---- | ---------------------------------------------------------------------- |
| 輸入 | logic flow 文件路徑（一次一個）                                        |
| 輸出 | `docs/todo/<name>.md`（檔名沿用 flow 檔名，`docs/todo/` 不存在則建立） |

## 執行流程

### Step 1：讀 logic flow

完整讀完輸入文件，**逐章節掃描**並標記功能點：

| 來源章節       | 拆成 todo 的對象                                   |
| -------------- | -------------------------------------------------- |
| 入口           | 每種「怎麼被打開」（直接 URL / 側欄 / 守衛導入等） |
| 使用者操作     | 每個 handler 觸發點                                |
| 呼叫的 API     | 每個 endpoint 對應的功能（不寫 status code）       |
| 做了什麼判斷   | 每條條件分支（if / else 各一條）                   |
| 前端條件渲染   | 每個顯示判斷                                       |
| 成功後做什麼   | 每個成功路徑的後續動作                             |
| 順帶會發生的事 | 每個全域行為（loading / toast / log / confirm）    |

> flow 文件中 `>` 引用標註「未實作 / 純 UI / 未使用」的項目**一樣列進來**，不要砍。

### Step 2：按功能群組分段

把所有 todo 按「功能子群組」分段，**群組由 flow 內容自然推導**，不是把 flow 的分析章節（入口 / 使用者操作 / ...）原封照搬。

常見群組推導邏輯：

- **CRUD feature**（list / create / update / delete）→ 每個動作各一組
- **流程 feature**（如登入 / 註冊 / 結帳）→ 按使用者旅程分段
- **跨功能行為**（loading / 攔截器 / 路由守衛）→ 集中放「全域行為」或「進出頁面」

群組名用口語（範例：「載入清單」「新增」「批量刪除」「進出頁面」「全域行為」），**不准用工程術語**（`Golden Path`、`Side Effect`、`Guard`、`Interceptor`）。

### Step 3：全部標 `[ ]`

所有 todo 項目預設未勾選 `[ ]`。**不主動判斷是否實作**——打勾是後續使用者帶對標檔案請你比對時才做的事，本 skill 不負責。

### Step 4：寫入檔案

寫到 `docs/todo/<name>.md`。

---

## 硬規則

1. **一條 todo = 一個動作 + 一個可觀察結果**
    - ✅ `[ ] 搜尋框輸入文字 → 自動帶 todoTitle 重新載入列表`
    - ❌ `[ ] 搜尋`（太籠統，看不出在驗證什麼）
    - ❌ `[ ] 點 Sign In`（沒結果）

2. **操作層級條列，不疊加**
    - 一個操作對應一條，**不要兩個動作擠一條**
    - ✅ 兩條：`[ ] 送出成功 → toast 成功訊息`、`[ ] 送出成功 → 關閉 Dialog 並重載列表`
    - ❌ 一條：`[ ] 送出成功 → toast + 關 Dialog + 重載列表`（三個獨立結果擠一起）
    - 例外：**不可分割的連動**可放一條（如「點開新增 Dialog → Dialog 內表單為空」）

3. **按功能群組分段，不要平鋪**
    - 用 `## 群組名` 分段
    - 群組由 flow 內容推導（CRUD / 旅程 / 跨功能），**不是照搬 flow 的章節名**
    - 群組名用口語

4. **無連結、無證據、無模式**
    - 純文字條列，**不附** `file:///` 連結（差別化於 QA checklist）
    - 不寫「證據在哪裡看」（loading 在哪、toast 怎麼觀察）
    - 沒有 api / web / full 模式之分

5. **全部 `[ ]` 未勾選**
    - 不主動判斷是否實作
    - 不要去 grep 程式碼
    - 純粹從 flow 抽出條目

6. **flow 中標註「未實作 / 純 UI / 未使用」的也列**
    - flow 文件用 `>` 引用標註「目前無實作 / 純 UI / 未使用」的 → 一樣列，**不額外標記**
    - 整體呈現「這個 feature 有哪些功能點」，不區分「做了沒」（那是對標比對的事）

7. **條件分支 if / else 各一條**
    - flow 寫「`isComplete === 'Y'` → 顯示綠色」「否則 → 顯示黃色」→ 拆兩條
    - 不要寫「狀態 Tag 顯示綠或黃」（看不出兩個分支都要驗）

8. **章節名與條目用口語**
    - 群組名禁用：`Golden Path` / `Side Effect` / `Guard` / `Edge Case` / `Interceptor`
    - 條目避免直接寫變數名（讀者不一定看過程式碼）——例外：API code、HTTP method、URL 路徑 OK 保留
    - ✅ `[ ] API 回成功（code === 2000）→ 顯示列表`
    - ✅ `[ ] 列表狀態欄：已完成 → 綠色標籤`
    - ❌ `[ ] data?.code === 2000 → todos = data.data`（直接抄程式碼）

---

## 輸出模板

```markdown
# [功能中文名稱] - Todo 清單

> **來源 flow**：docs/features/<name>.md
> **產出時間**：YYYY-MM-DD
> **使用方式**：之後請我帶對標檔案（如實作程式碼路徑、commit 範圍）比對，會逐條更新打勾狀態。

## [群組 1：例如「載入清單」]

- [ ] [動作或觸發條件] → [預期結果]
- [ ] ...

## [群組 2：例如「新增」]

- [ ] ...

## [群組 N：例如「全域行為」]

- [ ] ...
```

---

## 注意事項

- **群組數量沒上限也沒下限**：根據 flow 內容自然分。CRUD feature 通常 5-8 組，單一流程 feature 通常 3-5 組
- **不要重複 logic flow 的細節解釋**：flow 寫「`code === 2000` 才寫 cookie」→ todo 寫「登入成功 → 寫入 token cookie」即可
- **完整範例見** [examples.md](examples.md)
