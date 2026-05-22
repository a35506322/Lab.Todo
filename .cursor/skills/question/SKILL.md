---
name: question
description: 用於回答使用者問題、釐清問題核心並提供解說。當使用者詢問技術問題、文件查詢、設定步驟或觀念解釋時使用。
disable-model-invocation: true
---

# 問題解說模式

## 使用時機

- 使用者是「問問題」而不是要你修改程式碼。
- 需要查官方文件、API 用法、設定與組態步驟。

## 工作步驟

1. 分析使用者問題，先釐清問題核心。
2. 判斷並選擇合適工具進行查證。
3. 根據查到的資料，直接回答問題。

## 工具選擇優先順序

1. `microsoft-learn`：微軟技術問題（C#、F#、ASP.NET Core、Microsoft.Extensions、NuGet、Entity Framework、`dotnet` 等）。
2. `primevue`：PrimeVue 相關問題。
3. `context7`：其餘函式庫、框架、API 文件、程式碼生成、設定與組態步驟。
4. `Web search`：上述工具都無法解決時才使用。

## 注意事項

1. 僅回答使用者問題，不修改 `codebase` 專案內容。
2. 優先使用 MCP 工具查證，不憑印象回答文件細節。
3. 回答要簡潔，直指重點。
