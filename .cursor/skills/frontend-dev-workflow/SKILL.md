---
name: frontend-dev-workflow
description: 在編寫前端專案新功能、修復錯誤或重構程式碼時運用此技能。
---

# 前端開發工作流程規範

在編寫前端新功能、修復錯誤或重構程式碼時運用此技能。

## 前端開發核心原則

1. 先寫 UI 在寫程式碼
2. 串接 API 前先確認 API 規格可查看相關 API 專案的 Request/Response 規格

## 前端開發工作流程步驟

**步驟一：撰寫 UI 設計**

使用 PrimeVue 的元件庫進行 UI 設計，並使用假資料進行綁定

現行 Dashboard Template

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  ☰   [Logo] SAKAI                              📅   ✉️   👤               │
├──────────────┬──────────────────────────────────────────────────────────────┤
│              │                                                              │
│   HOME       │  [Page Name]                                                 │
│   ─────      │                                                              │
│   🏠 Dashboard│ 主內容區 (Page Content)                                     │
│              │                                                              │
│   TODO       │                                                              │
│   ─────      │                                                              │
│   📋 Todo List ◀──                                                         │
│   (選中)     │                                                               │
│              │                                                              │
│              │                                                              │
│              │                                                              │
└──────────────┴──────────────────────────────────────────────────────────────┘

```

常用 CRUD Template (PageContent)

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                         │
│  Manage Products                                              [Search...        🔍]     │
│                                                                                         │
│  [+ New]  [Delete]                                                            [Export]  │
│                                                                                         │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │ Actions  □   Code ↑↓    Name ↑↓      Image    Price ↑↓   Category ↑↓  Reviews ↑↓  Status ↑↓ │  │
│  ├───────────────────────────────────────────────────────────────────────────────────┤  │
│  │ ✏️  🗑️   □   f230fh0g3  Bamboo Watch  [圖]    $65.00     Accessories   ★★★★★    INSTOCK    │  │
│  │ ✏️  🗑️   □   nvklal433  Black Watch   [圖]    $72.00     Accessories   ★★★★☆    INSTOCK    │  │
│  │ ✏️  🗑️   □   zz21cz3c1  Blue Band     [圖]    $79.00     Fitness       ★★★☆☆    LOWSTOCK   │  │
│  │ ✏️  🗑️   □   244wgerg2  Blue T-Shirt  [圖]    $29.00     Clothing      ★★★★★    INSTOCK    │  │
│  │ ✏️  🗑️   □   h456wer53  Bracelet      [圖]    $15.00     Accessories   ★★★★☆    INSTOCK    │  │
│  │ ✏️  🗑️   □   av2231fwg  Brown Purse   [圖]    $120.00    Accessories   ★★★★☆    OUTOFSTOCK │  │
│  │ ✏️  🗑️   □   bib36pfvm  Chakra Bracelet[圖]   $32.00     Accessories   ★★★☆☆    LOWSTOCK   │  │
│  │ ✏️  🗑️   □   mbvjkgip5  Galaxy Earrings[圖]   $34.00     Accessories   ★★★★★    INSTOCK    │  │
│  │ ✏️  🗑️   □   vbb124btr  Game Controller[圖]   $99.00     Electronics   ★★★☆☆    LOWSTOCK   │  │
│  │ ✏️  🗑️   □   cm230f032  Gaming Set    [圖]    $299.00    Electronics   ★★★★☆    INSTOCK    │  │
│  └───────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                         │
│  [<<] [<]  [1] [2] [3]  [>] [>>]              Showing 1 to 10 of 30 products  [10 ▼]    │
│                                                                                         │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

CRUD Dialog Template

```
+------------------------------------------+
|  新增待辦事項                        [X] |
|------------------------------------------|
|                                          |
|  待辦標題 *                              |
|  [________________________]              |
|  (驗證錯誤: 待辦標題為必填)              |
|                                          |
|  待辦內容                                |
|  [________________________]              |
|  [________________________]              |
|  [________________________]              |
|                                          |
|------------------------------------------|
|                        [取消]  [儲存]    |
+------------------------------------------+
```

常用的 PrimeVue 的元件:

- Button
- InputText
- Select
- DataTable
- Card
- Toast
- ConfirmDialog
- Dialog

**步驟二：查看 API 規格**

1. 查閱 src/TodoAPI/Module 相關的 API 規格

**步驟三：撰寫程式碼**

1. 按照專案的 rule / skill 進行開發

**注意事項**

1. 綁定表單驗證並使用 zod 進行驗證 (注意後端 API 的 Request 驗證需統一)

## 成功指標

1. `npm run format` -> `npm run build` 成功建置專案即可
2. 如果有 eslint 或 prettier 的錯誤，可不予理會
