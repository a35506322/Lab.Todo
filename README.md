# Lab 專案

此專案為演示前後端分離範本專案及 AI 輔助開發工具使用<br>
目前所有的 AI 工具基本都大同小異，應都已實裝 MCP、Rule、Skill、AGENTS.md 等規範與工具，在自行按照工具調整即可

## 環境

- .NET 10.0
- Cursor 3.31 (主要)
- Claude Code Desktop (輔助)
- Node.js 22.21.1+

---

## 專案介紹

| 專案                    | 說明                                | README                                     |
| ----------------------- | ----------------------------------- | ------------------------------------------ |
| TodoAPI                 | .NET 10.0 垂直切割 Minimal API 專案 | [TodoAPI 專案架構](src/TodoAPI/README.md)  |
| TodoAPI.IntegrationTest | .NET 10.0 整合測試專案              |                                            |
| TodoAPI.UnitTest        | .NET 10.0 單元測試專案              |                                            |
| TodoWeb                 | VITE + VUE 3 前端專案               | [TodoWeb 專案架構](src/todo-web/README.md) |

---

## 如何開發

1. 使用技能 `grill-with-docs` 討論需求
2. 實作，就這麼簡單
3. 複雜需求：討論完後依序使用技能 `to-spec`（整理成規格）→ `to-issues`（拆成垂直切片 issue）→ `implement-spec`（依 issue 相依關係平行實作）
    - `implement-spec` 會一次開多個 subagent，各自在獨立的 worktree 平行實作，最後再跑 `code-review`，花費較高
    - 中型需求改用 `implement` 逐一實作 issue 即可，較省

## AI 工具

### 使用建議

1. 不想讓 `Cursor` 讀取的檔案，加進 `.cursorignore` 或 `.gitignore`。
2. 基本的 `Rule` 放在 `.cursor/rules`，`Skill` 放在 `.agents/skills`，可以依個人習慣調整。使用 Claude Code Desktop 的話，執行 `scripts/` 裡的同步腳本即可共用同一套設定。
3. 個別專案的 `Rule`（例如團隊規範、重要的商業邏輯）放進對應的資料夾，例如 `TodoAPI` 放 `.cursor/rules/TodoAPI/`，其他專案以此類推。
4. 常用、又會重複做的修改可以包成 `Skill` 方便重用。用 `writing-for-agents` 技能就能很快產生 `Skill` 檔案。

### 好用的 Skill

| Skill             | 用途                                               | 使用時機                                       |
| ----------------- | -------------------------------------------------- | ---------------------------------------------- |
| `grill-with-docs` | 反覆追問釐清需求，同時產生 ADR 與名詞表            | 開發新功能前                                   |
| `microsoft-docs`  | 查詢 Microsoft 官方文件（.NET、Azure 等）          | 遇到 .NET 相關問題時                           |
| `commit-msg`      | 依 staged 變更產生條列式的詳細 commit message      | 準備 commit 前（需先 `git add`）               |
| `handoff`         | 將目前對話整理成交接文件，供新的 session 接手      | context 用量達 50% 左右時，換新 session 繼續做 |

### MCP 工具

**已設定好 MCP 工具** 只要確保 AI 每次讀取都是官網維護的資料基本都不會有什麼太大問題

```json
{
    "mcpServers": {
        "Context7": {
            "url": "https://mcp.context7.com/mcp",
            "modes": ["ask", "agent"]
        },
        "microsoft-learn": {
            "type": "http",
            "url": "https://learn.microsoft.com/api/mcp",
            "modes": ["ask", "agent"]
        },
        "primevue": {
            "command": "npx",
            "args": ["-y", "@primevue/mcp"],
            "modes": ["ask", "agent"]
        }
    }
}
```
