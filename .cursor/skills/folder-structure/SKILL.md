---
name: folder-structure
description: 依照使用者指定的檔案與資料夾輸出樹狀結構，並可在節點旁加上註解說明。當使用者要求產生專案結構、目錄樹或檔案架構時使用。
disable-model-invocation: true
---

# 產生資料夾結構

## 使用時機

- 使用者指定某個模組、資料夾或專案，要你整理結構。
- 使用者希望用樹狀結構快速理解檔案配置。

## 工作步驟

1. 讀取使用者指定的資料夾與檔案範圍。
2. 以樹狀結構輸出目錄。
3. 在資料夾或檔案旁加上 `#` 註解（若使用者有要求）。
4. 檔案過多時可省略部分內容，並保留核心路徑。

## 輸出格式

```text
Modules # 模組
├── Auth # 權限模組
│   ├── GetByString # 取得權限
│   │   ├── Endpoint.cs # Endpoint 檔案
│   │   ├── Models.cs   # Models 檔案
│   │   └── Examples.cs # Examples 檔案
│   └── UserController
```

## 參考範例

- 常見輸出範例請見 [examples.md](examples.md)。
