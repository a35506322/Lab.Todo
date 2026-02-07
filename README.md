# Lab 專案

此專案為演示前後端分離範本專案 + Cursor Rule 及 Skill 使用

## 環境

- .NET 10.0
- Cursor 2.4+
- Node.js 22.21.1+

---

## 如何開發

1. 整理文件

| 文件類型       | 說明                     |
| -------------- | ------------------------ |
| **使用者故事** | 從使用者角度描述功能需求 |
| **流程圖**     | 視覺化呈現業務流程       |
| **商業邏輯**   | 商業邏輯規則             |
| **會議紀錄**   | 條列式記錄討論與決策     |
| **UIUX 設計**  | 視覺化呈現 UIUX 設計     |

2. 設計 DB 規格
3. 設計 API 規格
4. 後端開發 API
5. 前端開發 UIUX

> ⚠️ 關鍵是在於整理文件，以及一開始的專案開發架構，只要架構完整，文件寫得自己能夠理解那麼 AI 才能夠幫助你更快完成開發

## AI 輔助開發 (Ask - Plan - Agent 模式)

1. 使用 `Ask` 模式跟 AI 討論需求和文件，並確認他真的理解需求
2. 使用`Plan` 模式，規劃實作步驟，並且仔細閱讀規劃書，反覆跟 AI 討論確認直到滿意為止
3. 切換到 `Agent` 模式，讓 AI 開始開發
4. 當成品滿意 80 趴左右，就可以自己接受完成後續，記得需要 Review 跟手動測試
5. 如需重構一些程式碼可以繼續 `Ask` -> `Plan` -> `Agent` 這個循環
6. 建議如果發現 AI 程式碼產生的都是錯的，直接 `Rollback` 重新開始，調整文件跟 `Prompt` 繼續，或者可以選擇高等模型如 `Opus4.6` 討論，實作建議直接使用 `Auto` 或著 `Composer` 模型進行實作

🤖 哪些可以交給 Agent？

| 工作項目     | 可否 Agent 開發  |
| ------------ | :--------------: |
| 整理文件     | 🟡 半自動化/手動 |
| 定義 DB 規格 | 🟡 半自動化/手動 |
| API 開發     |     ✅ Agent     |
| UIUX 設計    | 🟡 半自動化/手動 |
| 前端開發     |     ✅ Agent     |
| 單元測試     |     ✅ Agent     |
| 整合測試     |     ✅ Agent     |

開發後端時 Agent 可搭配 TDD 模式 **（可選）**

本專案已內建 TDD 相關 Skill：

```
.cursor/skills/
├── backend-tdd-workflow/     # TDD 開發流程
├── mstest-unit-test/         # 單元測試
└── mstest-integration-test/  # 整合測試
```

**想要啟用 TDD？** → 讓 Agent 讀取 `backend-tdd-workflow/SKILL.md`

**不想用 TDD？** → 從 `AGENTS.md` 中移除相關 `skill` 即可

> ⚠️ 以上開發流程僅供參考，可以依據個人喜好調整，但其實會建議在開發時候最重要的是文件以及設定 Agent 的規則跟技能，要讓 Agent 知道什麼是什麼，這樣才能讓 Agent 幫助你更快完成開發。

---

## 專案介紹

| 專案                    | 說明                                | README                                     |
| ----------------------- | ----------------------------------- | ------------------------------------------ |
| TodoAPI                 | .NET 10.0 垂直切割 Minimal API 專案 | [TodoAPI 專案架構](src/TodoAPI/README.md)  |
| TodoAPI.IntegrationTest | .NET 10.0 整合測試專案              |                                            |
| TodoAPI.UnitTest        | .NET 10.0 單元測試專案              |                                            |
| TodoWeb                 | VITE + VUE 3 前端專案               | [TodoWeb 專案架構](src/todo-web/README.md) |

---

## AI 工具

### 使用建議

1. 如果有不想要被 `Cursor` 讀取的檔案，可以加入 `.cursorignore` 檔案中
2. `.cursor` 資料夾已有設定基本 `Rule` 跟 `Skill`，可以隨個人喜好而去調整
3. 如果有各項專案的 `Rule` 如 `TodoAPI` 專案，可以加入 `.cursor/rules/TodoAPI` 資料夾中 (以次類推)，比如些商業邏輯
4. `AGENTS.md` 屬於每次對話必讀的，已設定每次都必須讀 `Rule` 跟 `Skill`
5. 能將重複且常用邏輯的變更包裝成 `Skill`，方便下次使用
6. 有寫一些常用 `slash command` 可以參考，產生專案技能檔案如 `create-project-skill`，自己可以按自己需求調整或增加

### MCP 工具

```json
{
    "mcpServers": {
        "context7": {
            "url": "https://mcp.context7.com/mcp",
            "headers": {
                "CONTEXT7_API_KEY": ""
            }
        },
        "microsoft-learn": {
            "name": "microsoft-learn",
            "type": "http",
            "url": "https://learn.microsoft.com/api/mcp",
            "headers": {}
        },
        "primevue": {
            "command": "npx",
            "args": ["-y", "@primevue/mcp"]
        }
    }
}
```
