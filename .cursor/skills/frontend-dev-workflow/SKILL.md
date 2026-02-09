---
name: frontend-dev-workflow
description: 在編寫前端專案新功能、修復錯誤或重構程式碼時運用此技能。
---

# 前端開發工作流程規範

在編寫前端新功能、修復錯誤或重構程式碼時運用此技能。

## 使用場景

-   當需要編寫/修改前端功能

## 前端開發核心原則

1. 了解使用者需求
2. 了解 API 規格實際作用
3. 先寫 UI 在寫邏輯程式碼

## 前端開發工作流程步驟

**步驟二：查看 API 規格**

1. 查閱 src/TodoAPI/Module 相關的 API 規格

重點如下:

-   Endpoint URL : 如 /todo/getTodoById
-   Endpoint 描述 : 知道此 API 的用途與功能
-   Request 規格 : 了解 API 的 Request 參數與格式
-   Response 規格 : 了解 API 的 Response 格式與內容
-   Example 範例 : 了解 API 回傳各種情況

**步驟二：撰寫 UI 設計**

可查看 `.cursor/skills/uiux-dev/SKILL.md` 的 UIUX 設計規範

**步驟三：撰寫程式碼**

1. 按照專案的 rule 進行開發
2. 使用對應的 skill 進行開發

**注意事項**

1. 綁定表單驗證並使用 zod 進行驗證 (注意需與後端 API 的 Request 驗證一模一樣)

## 成功指標

1. `cd src/todo-web` -> `npm run format` -> `npm run build` 成功建置專案即可
2. 如果有 eslint 或 prettier 的錯誤，可不予理會
