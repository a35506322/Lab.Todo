---
name: uiux-dev
description: 在編寫 UIUX 設計時運用此技能。
---

# UIUX 開發規範

在編寫 UIUX 設計時運用此技能。

## UIUX 開發核心原則

1. 理解 API 規格了解 Request/Response 規格
2. 將 API 規格轉換為 UIUX 設計
3. 使用 PrimeVue 的元件庫進行 UIUX 設計
4. 使用 mock 資料進行綁定
5. 以簡單的 UIUX 設計為主，不要過度複雜

## UIUX 開發工作流程步驟

**步驟一：理解 API 規格**
查閱 src/TodoAPI/Module 相關的 API 規格

重點如下:

- Endpoint URL : 如 /todo/getTodoById
- Endpoint 描述 : 知道此 API 的用途與功能
- Request 規格 : 了解 API 的 Request 參數與格式
- Response 規格 : 了解 API 的 Response 格式與內容
- Example 範例 : 了解 API 回傳各種情況

**步驟二：將 API 規格轉換為 UIUX 設計**

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

如需更多元件，可查看 PrimeVue 的元件庫
