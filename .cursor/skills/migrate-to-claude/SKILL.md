---
name: migrate-to-claude
description: 將專案內 `.cursor` 規範與資產 1:1 遷移到 Claude Code 可讀取的 `.claude` 結構，僅做必要格式對齊、不改語意。當使用者提到「migrate to claude」、「.cursor 轉 .claude」、「Cursor 規範搬到 Claude」時使用。
disable-model-invocation: true
---

# Migrate to Claude (1:1)

## 目標

只做專案內 1:1 遷移：把 `.cursor` 既有內容對應到 Claude Code 專案層級檔案，保留原文與原本限制，不做重構、不做語意優化。

## 強制限制

1. 只可修改當前專案。
2. 禁止動到 `~/.claude/*`。
3. 禁止擅自改寫規範語氣與約束條件。
4. 若無直接對應格式，先保留原文，再加最小必要包裝（例如 frontmatter）。

## Context7 先查規範（必做）

每次遷移前先用 Context7 查 Claude Code 最新規範，至少確認：

1. 設定層級：`.claude/settings.json`、`.claude/settings.local.json`。
2. 規範檔位置：`.claude/rules/**/*.md` 與 `CLAUDE.md`（官方參考：`https://code.claude.com/docs/zh-TW/memory`（`CLAUDE.md` 與 `.claude/rules/`）。
3. 指令位置：`.claude/commands/*.md`。
4. 技能位置：`.claude/skills/<skill>/SKILL.md`。
5. 子代理位置：`.claude/agents/*.md`（需要 YAML frontmatter）。

## 1:1 遷移對照

- `.cursor/settings.json` → `.claude/settings.json`
- `.cursor/settings.local.json` → `.claude/settings.local.json`
- `.cursor/rules/**/*.mdc` → `.claude/rules/**/*.md`（目錄結構與檔名 1:1；副檔名改為 `.md`）
- `CLAUDE.md` 或 `.claude/CLAUDE.md` → 作為 rules 索引入口（列出 `.claude/rules/*`）
- `.cursor/commands/*.md` → `.claude/commands/*.md`
- `.cursor/skills/<name>/*` → `.claude/skills/<name>/*`
- `.cursor/agents/*.md` → `.claude/agents/*.md`（補齊 frontmatter）

## 執行流程

1. 盤點 `.cursor` 檔案清單。
2. 依對照表建立 `.claude` 目錄結構。
3. 逐檔複製內容（保留原文）。
4. 產生 `CLAUDE.md` 或 `.claude/CLAUDE.md` 作為 rules 入口索引。
5. 只補最小必要格式（例如 agent frontmatter）。
6. 檢查是否有應加入 `.gitignore` 的本地檔（如 `.claude/*.local.json`）。
7. 回報「已遷移 / 無對應 / 需人工決策」三類清單。

## 回報格式

- 已遷移：`來源` → `目的`
- 無對應：`來源`（原因）
- 需人工決策：`來源`（衝突點與建議）
