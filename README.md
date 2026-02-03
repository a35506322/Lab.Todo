# Lab 專案

此專案為演示前後端分離範本專案 + Cursor Rule 及 Skill 使用

## 環境

-   .NET 10.0
-   Node.js 22.13.0
-   Cursor 2.4+

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
        }
    }
}
```

## 專案介紹

| 專案    | 說明                                | README                                    |
| ------- | ----------------------------------- | ----------------------------------------- |
| TodoAPI | .NET 10.0 垂直切割 Minimal API 專案 | [TodoAPI 專案架構](src/TodoAPI/README.md) |

## 使用 AI Tools 建議

1. 如果有不想要被 Cursor 讀取的檔案，可以加入 `.cursorignore` 檔案中
2. .cursor 已有設定基本 Rule 跟 Skill，可以隨個人喜好而去調整
3. 如果有各項專案的 Rule 如 TodoAPI 專案，可以加入 `.cursor/rules/TodoAPI` 資料夾中 (以次類推)
4. `AGENTS.md` 屬於每次對話必讀的，已設定每次都必須讀 Rule 跟 Skill
5. 能將重複且常用邏輯的變更包裝成 Skill，方便下次使用
