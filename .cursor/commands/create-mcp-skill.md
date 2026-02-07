# 使用 MCP 讀取文檔並創建技能

## MCP 工具

1. microsoft-learn
2. primevue
3. Context7

## 步驟說明

1. 按照使用者需求，使用 MCP 工具讀取相關文檔
2. 分析文檔內容，創建/修改 Skill 指令檔案

**資料夾結構**

```
.cursor/skills/
├── {output-name}/
│   └── SKILL.md # 技能指令檔案
```

**Skill 指令檔案範例**

`````markdown
---
name: skill-name
description: [能讓 AGENT 讀取描述就能知道 Skill 的用途]
---

# [Skill Title]

[能讓 AGENT 讀取描述就能知道 Skill 的用途]

## [Skill1 Name]

- 能以清單方式列出 Skill 的步驟
- 能舉出範例程式碼 (ex: ````csharp)，並適當舉出正反面案例

## [Skill2 Name]

- 能以清單方式列出 Skill 的步驟
- 能舉出範例程式碼 (ex: ````csharp)，並適當舉出正反面案例

...
`````

**寫作指南**

- 不要逐字複製文件；要根據使用者的閱讀習慣進行改寫。
- 專注於使用模式和程式碼範例
- 去除冗餘訊息，保留關鍵訊息
- 將大型主題拆分成單獨的技能
- 務必提供可運行的程式碼範例
- 不僅要說明如何使用，還要說明何時使用以及為什麼使用。
