# 完整範例

> 下面是從 `docs/features/todo.md` 產出的 `docs/todo/todo.md` 完整內容，作為**目標長相**參考。
>
> 範例為 Vue 3 + .NET 的 Todo CRUD，但模板與規則框架無關。

---

```markdown
# 待辦事項管理 - Todo 清單

> **來源 flow**：docs/features/todo.md
> **產出時間**：2026-05-24

## 載入清單

- [ ] 進入 /todo 頁面 → 自動載入全部 todo
- [ ] 搜尋框輸入文字 → 自動帶 todoTitle 重新載入
- [ ] 搜尋字串前後空白 → trim 後若為空則不帶 todoTitle 參數
- [ ] 狀態篩選切換 → 自動帶 isComplete（Y / N）重新載入
- [ ] 篩選為「全部」（null）→ 不帶 isComplete 參數
- [ ] API 回成功（code === 2000 且有 data）→ 顯示列表
- [ ] API 回非 2000 → 列表清空
- [ ] API 失敗 → 列表清空（錯誤 toast 由攔截器處理）

## 列表顯示

- [ ] 內容欄空值 → 顯示 `-`
- [ ] 時間欄 → 用 formatTWDateTime 格式化
- [ ] 狀態 Tag：isComplete = 'Y' → 綠色「已完成」
- [ ] 狀態 Tag：isComplete = 'N' → 黃色「未完成」

## 新增

- [ ] 點「新增」按鈕 → 開啟新增 Dialog
- [ ] Dialog 標題顯示「新增待辦事項」
- [ ] 新增模式不顯示「是否完成」欄位
- [ ] 標題空白送出 → 顯示必填錯誤、不打 API
- [ ] 標題超過 100 字 → 顯示長度錯誤
- [ ] 內容超過 500 字 → 顯示長度錯誤
- [ ] 表單驗證未通過 → 不打 API
- [ ] 送出成功（code === 2000）→ 顯示成功 toast
- [ ] 送出成功 → 關閉 Dialog 並重置表單
- [ ] 送出成功 → 自動重載列表
- [ ] 送出回非 2000 → 不 toast、不關 Dialog（靜默）

## 編輯

- [ ] 點列上鉛筆 → 開啟編輯 Dialog 並帶入該筆資料
- [ ] Dialog 標題顯示「編輯待辦事項」
- [ ] 編輯模式顯示「是否完成」Select
- [ ] 標題空白送出 → 顯示必填錯誤、不打 API
- [ ] 標題超過 100 字 → 顯示長度錯誤
- [ ] 內容超過 500 字 → 顯示長度錯誤
- [ ] 是否完成必須是 Y 或 N
- [ ] 表單驗證未通過 → 不打 API
- [ ] 送出成功（code === 2000）→ 顯示成功 toast
- [ ] 送出成功 → 關閉 Dialog 並重置表單
- [ ] 送出成功 → 自動重載列表
- [ ] 送出回非 2000 → 不 toast、不關 Dialog

## Dialog 取消

- [ ] Dialog 按取消或右上 X → 關閉 Dialog（不送 API）

## 單筆刪除

- [ ] 點列上垃圾桶 → 跳出 Confirm 對話框
- [ ] Confirm 按取消 → 不執行任何動作
- [ ] Confirm 按接受 → 打 DELETE API
- [ ] 刪除成功（code === 2000）→ 顯示成功 toast
- [ ] 刪除成功 → 自動重載列表

## 批量刪除

- [ ] 列表 checkbox 可多選
- [ ] 未選任何 todo → 批量刪除按鈕 disabled
- [ ] 點批量刪除按鈕（已選一筆以上）→ 跳出 Confirm
- [ ] Confirm 按取消 → 不執行任何動作
- [ ] Confirm 按接受 → 對選取列全部並行打 DELETE
- [ ] 全部刪除完成 → 顯示成功 toast（含筆數）
- [ ] 全部刪除完成 → 清空選取狀態
- [ ] 全部刪除完成 → 自動重載列表

## 進出頁面

- [ ] 直接輸入 /todo URL → 進入待辦頁
- [ ] 從側欄「Todo List」點擊 → 進入待辦頁
- [ ] 未登入訪問 /todo → 錯誤 toast 並導向 /login?redirect=/todo

## 全域行為

- [ ] 任何 API 呼叫進行中 → 顯示全螢幕 loading 遮罩
- [ ] 有 token 時打 API → 自動帶 Authorization Bearer header
- [ ] API 回 401 → 清除 token 並導向 /login
- [ ] API 回 401 / 403 / 422 / 500 → 顯示對應錯誤 toast
- [ ] 無回應 → 顯示「網路連線異常」toast
- [ ] 請求逾時 → 顯示「請求逾時」toast
- [ ] 攔截器接到錯誤 → console.error 記錄
```

---

## 群組推導對照

| flow 章節    | examples 對應群組                                            |
| ------------ | ------------------------------------------------------------ |
| 入口         | 「進出頁面」                                                 |
| 使用者操作   | 拆到各 CRUD 群組（「新增」「編輯」「單筆刪除」「批量刪除」） |
| 呼叫的 API   | 不另成群組（API 動作已散在各 CRUD 群組）                     |
| 做了什麼判斷 | 拆到對應 CRUD / 「載入清單」群組                             |
| 前端條件渲染 | 「列表顯示」「Dialog 取消」                                  |
| 成功後做什麼 | 拆到對應 CRUD 群組的「成功 →」條目                           |
| 副作用       | 「全域行為」（loading / toast / confirm / log / token 攔截） |

> 同樣是「按 flow 推導群組」，**單一流程 feature**（如登入）會長這樣：`正常登入` / `表單檢查` / `登入失敗` / `進出頁面` / `全域行為` — 不會出現 CRUD 群組。
