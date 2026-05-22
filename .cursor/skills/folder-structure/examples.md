# 資料夾結構範例

## 範例 1：後端 API 模組

```text
Modules # API 模組
├── Todo # 待辦模組
│   ├── CreateTodo # 建立待辦
│   │   ├── Endpoint.cs # Endpoint
│   │   ├── Models.cs   # Request/Response
│   │   └── Examples.cs # Swagger 範例
│   ├── UpdateTodo # 更新待辦
│   │   ├── Endpoint.cs
│   │   └── Models.cs
│   └── DeleteTodo # 刪除待辦
│       └── Endpoint.cs
└── Auth # 權限模組
    └── GetByString
```

## 範例 2：前端 Vue 功能頁

```text
src # 原始碼
├── views # 頁面
│   └── todo
│       ├── TodoListView.vue # 列表頁
│       └── TodoDetailView.vue # 明細頁
├── components # 共用元件
│   └── todo
│       ├── TodoFormDialog.vue # 新增/編輯彈窗
│       └── TodoTable.vue # 待辦表格
├── stores
│   └── todo.ts # Pinia 狀態管理
└── api
    └── todo.ts # Todo API 封裝
```
