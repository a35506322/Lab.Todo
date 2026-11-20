---
name: commit-msg
description: Drafts 條列式 commit messages from staged git changes only. Use when the user asks for a commit msg、commit message、條列式 commit，或只要訊息不要先 commit。
---

# Commit msg（條列式）

產出可直接貼上的 commit message。預設**只給訊息、不執行 commit**；使用者明確說「幫我 commit」才進入實際提交。

**範圍：只看暫存區（staged）**。未 stage 的工作區變更忽略；暫存為空則回報並停下，不改看 unstaged。

## Steps

1. **讀暫存** → verify: 已跑 `git status`、`git diff --cached`、`git log -5 --oneline`；訊息只依 `--cached`；暫存為空則告知使用者先 `git add`。
2. **定 type + 標題** → verify: 標題一行、`type: 中文摘要`；type 對齊本 repo 慣例（`feat` / `fix` / `style` / `chore` / `refactor` / `docs` / `test`）；摘要寫「為什麼／結果」，不堆檔名清單。
3. **寫條列 body** → verify: 空一行後每點一條 `- `；每條一個具體改動；只涵蓋 staged diff 意圖，無無關廢話。
4. **交件** → verify: 回覆只有訊息本體（可包在 code fence），未擅自 `git commit`。

## Format

```
type: 一句中文摘要

- 具體改動 1
- 具體改動 2
- 具體改動 3
```

## Example

```
style: 修 Toast / Login 與表格標題殘留樣式

- CustomToast 改 Tailwind flex，保留 severity icon 與換行邏輯
- Login 縮減卡片間距、字級與輸入框 padding，Logo 改 h-* 控制尺寸
- DataTable 標題統一 mb-0!，避免標題與搜尋列垂直跑版
- 移除 main.css 中已不用的 v3 surface / color CSS var bridge
- 少數 p-error 改為 text-red-500
```

## Rules

- 條列優於長段落；單點改動可只有標題、省略 body。
- 不編造未出現在 `git diff --cached` 的改動；不把 unstaged 寫進訊息。
- 不提交 secrets（`.env`、credentials 等）；若使用者堅持，先警告。
- 實際 commit 時遵守既有 git 安全協議（不改 config、不 `--no-verify`、不 force push main）。
