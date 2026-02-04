---
name: backend-coverage-report
description: 產生後端專案的覆蓋率測試報告。使用時機：使用者要求產生覆蓋率報告、檢視測試覆蓋率、或討論 coverage 時。
---

# 後端覆蓋率測試報告

產生 .NET 後端專案的程式碼覆蓋率 HTML 報告，產出於 `coveragereport/index.html`。需在 **PowerShell** 環境或透過 VS Code 任務執行。

## 使用時機

-   使用者要求產生覆蓋率報告 時，讀取本 Skill 並依步驟執行。

## 方式一：在 VS Code 內執行（推薦）

-   從 VS Code **終端機 → 執行工作** 或命令面板 **Tasks: Run Task** 選擇任務 `test:coverage:report`。
-   該任務會依序執行：`test_coverage:todoapi_unit_test`（收集覆蓋率）→ `report:coverage`（產生 HTML）。
-   報告產出於工作區根目錄 `coveragereport/index.html`，用瀏覽器開啟即可。

## 方式二：在終端機手動執行（PowerShell）

-   必須使用 **PowerShell**（腳本為 `.ps1`，且任務內以 `powershell -ExecutionPolicy Bypass -File` 呼叫）。

-   步驟 1：執行測試並收集覆蓋率（請依專案替換測試專案路徑）：

```powershell
dotnet test test/TodoAPI.UnitTest/TodoAPI.UnitTest.csproj --collect "XPlat Code Coverage"
```

-   步驟 2：執行報告腳本：

```powershell
powershell -ExecutionPolicy Bypass -File ".cursor/skills/backend-coverage-report/scripts/report-coverage.ps1"
```

-   報告產出於 `coveragereport/index.html`。

**正確**：在 PowerShell 中依序執行上述兩段；工作目錄為專案根目錄。

**錯誤**：在 CMD 或 bash 直接執行 `.ps1`，或未先執行 `dotnet test --collect "XPlat Code Coverage"` 就執行報告腳本（會找不到 `coverage.cobertura.xml`）。

## 前置需求

-   **ReportGenerator**：報告由 [ReportGenerator](https://github.com/danielpalme/ReportGenerator) 產生。若本機未安裝，先執行：

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

-   **腳本位置**：`.cursor/skills/backend-coverage-report/scripts/report-coverage.ps1`，由 VS Code 任務 `report:coverage` 呼叫（`tasks.json` 內需指向此路徑）。

## 產出與路徑

-   覆蓋率來源：各測試專案下的 `test/**/TestResults/**/coverage.cobertura.xml`（腳本會依測試專案取最新一筆合併）。
-   報告目錄：工作區根目錄 `coveragereport/`，入口為 `coveragereport/index.html`。
