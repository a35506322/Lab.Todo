---
name: code-to-notes
description: Extracts knowledge from project source code into minimal markdown notes under docs/learn/. One concept per file with bullet points, minimal examples, and official doc URLs. Use when the user asks to document code patterns, summarize implementation, or distill source code into learn notes or 極簡筆記.
---

# Code → 極簡筆記

從專案程式碼萃取可重讀的極簡筆記，寫入 `docs/learn/{topic}/`。

## 流程

```
1. 讀程式碼 → 2. 萃取概念 → 3. 寫檔 → 4. 建 README 目錄
```

### 1. 讀程式碼

按優先順序：

1. 使用者指定的**檔案 / 資料夾**
2. 使用者提到的**功能或模組**→ 搜尋相關檔案
3. 整個專案掃描（僅限小專案或使用者明確要求）

**只讀有意義的程式碼**：跳過 node_modules、lock 檔、自動生成檔案。

### 2. 萃取概念

- **一個獨立概念 = 一個 `.md`**
- 概念 ≠ 檔案；多個檔案可能只產出一個概念筆記
- 檔名：`NN-短標題.md`（兩位數序號 + 中文或英文 kebab）

**值得記錄的：**

| 類型                 | 範例                             |
| -------------------- | -------------------------------- |
| 非顯而易見的 pattern | 自訂 composable、middleware 組合 |
| 專案特定的設定       | 為何選某種 auth 流程             |
| 常踩的坑             | 為何這樣寫而不那樣寫             |
| 跨檔協作             | 多檔案如何配合完成一功能         |

**不要記錄的：**

- 框架預設行為（官網有寫的）
- 標準 CRUD、boilerplate
- 程式碼逐行翻譯

### 3. 寫檔

每個 md 遵守 [template.md](template.md)：

- 標題一句話
- 條列重點（表格優先）
- **一個**最小示意範例（精簡後的，非原始碼複製貼上）
- **官網**區塊（相關 API / pattern 的完整 URL）
- 繁體中文、言簡意賅

**不要寫：**

- 原始碼大段複製
- 未在程式中體現的推測
- 使用者沒要求的額外檔案

### 4. 建 README

`docs/learn/{topic}/README.md`：

- 一句話說明來源（例：「從 `server/` 目錄萃取」）
- 目錄表（檔名 | 重點）
- 頂層官網連結（2–3 條）

## 品質檢查

- [ ] 每個重點檔 < 60 行（極簡）
- [ ] 每檔有官網 URL
- [ ] 範例是精簡示意，非原始碼照抄
- [ ] 沒有重複主題跨檔
- [ ] README 連結全部有效

## 範例產出

參考本專案：`docs/learn/nuxt/`、`docs/learn/supabase/`
