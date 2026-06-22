---
name: chat-to-notes
description: Extracts Cursor conversations into minimal markdown notes under docs/learn/. One topic per file with bullet points, short examples, and official doc URLs. Use when the user asks to encapsulate, summarize, or distill chat discussions into learn notes, 極簡筆記, or docs/learn documentation, or provides agent transcript UUIDs.
---

# Chat → 極簡筆記

把 Cursor 對話萃取成可重讀的極簡筆記，寫入 `docs/learn/{topic}/`。

## 流程

```
1. 讀來源 → 2. 拆主題 → 3. 寫檔 → 4. 建 README 目錄
```

### 1. 讀來源

按優先順序：

1. 使用者指定的 **agent transcript UUID** → 搜尋 `agent-transcripts/`
2. **當前對話**上下文
3. 專案既有程式 / ADR / CONTEXT.md（只取相關片段）

找不到 UUID 檔案時：用同專案含相同關鍵字的 transcript，並告知使用者。

### 2. 拆主題

- **一個獨立概念 = 一個 `.md`**
- 合併重複、刪掉寒暄與試錯過程
- 檔名：`NN-短標題.md`（兩位數序號 + 中文或英文 kebab）

### 3. 寫檔

每個 md 遵守 [template.md](template.md)：

- 標題一句話
- 條列重點（表格優先）
- **一個**最小可跑範例（除非 API 有多種用法）
- **官網**區塊（完整 URL，至少 1 條）
- 繁體中文、言簡意賅

**不要寫：**

- 對話逐字稿
- Agent 除錯過程
- 未在對話或程式中出現的推測
- 使用者沒要求的額外檔案

### 4. 建 README

`docs/learn/{topic}/README.md`：

- 一句話說明來源
- 目錄表（檔名 | 重點）
- 頂層官網連結（2–3 條）

## 品質檢查

- [ ] 每個重點檔 < 60 行（極簡）
- [ ] 每檔有官網 URL
- [ ] 範例對應本專案實際 stack
- [ ] 沒有重複主題跨檔
- [ ] README 連結全部有效

## 範例產出

參考本專案：`docs/learn/supabase/`
