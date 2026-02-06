# 創建專案 Skill 指令

創建專案 Skill 指令，方便 AGENT 重複使用。

## 創建步驟

### 步驟 1：分析使用者需求

**重複動作類型**

-   如重複創建第三方 API 的 Adapter 方便重複使用

### 步驟 2：讀取專案程式碼

-   將步驟 1 的分析結果，讀取專案內部相關程式碼，尋找可復用程式碼

### 步驟 3：確認創建位置

-   分析當前 `.cursor/skills` 資料夾結構，確認創建位置
-   創建新的 Skill 如 `.cursor/skills/api/SKILL.md`

```
.cursor
└── skills
    └── api
        └── SKILL.md
```

-   修改檔案內容，加入新的 Skill 描述

### 步驟 4：執行創建 / 修改 Skill

**創建 Skill 指令格式**

可以參考 Cursor 官網 [Skill 說明](https://cursor.com/zh-Hant/docs/context/skills)

`````markdown
---
name: skill-name
description: [能讓 AGENT 讀取描述就能知道 Skill 的用途]
---

# [Skill Title]

[能讓 AGENT 讀取描述就能知道 Skill 的用途]

## [Skill1 Name]

-   能以清單方式列出 Skill 的步驟
-   能舉出範例程式碼 (ex: ````csharp)，並適當舉出正反面案例

## [Skill2 Name]

-   能以清單方式列出 Skill 的步驟
-   能舉出範例程式碼 (ex: ````csharp)，並適當舉出正反面案例

...
`````

** 修改 Skill 指令檔案**

-   原則就是按照原本 Skill 指令檔案進行修改
