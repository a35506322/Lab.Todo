---
name: zod
description: 當使用 PrimeVue Form 時適用於編寫 Zod 時最佳實踐，包含 Zod 使用方式、Zod Schema 範例等。
---

# Zod 開發規範

使用 Zod 作為 PrimeVue Form 的驗證 schema。

## 使用場景

- 當使用 PrimeVue Form 時

## 基本範例

在 PrimeVue Form 中使用的常見 Zod Schema 範例

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

// 必填文字
z.string().trim().min(1, { error: "此欄位為必填" });

// Email
z.string().email({ error: "請輸入有效的 Email" });

// 密碼（複合規則）
z.string()
    .min(6, { error: "密碼至少 6 個字元" })
    .max(20, { error: "密碼最多 20 個字元" })
    .refine((v) => /[A-Z]/.test(v), { error: "需包含大寫字母" })
    .refine((v) => /[a-z]/.test(v), { error: "需包含小寫字母" })
    .refine((v) => /\d/.test(v), { error: "需包含數字" });

// 數字範圍
z.number().min(1).max(100);

// 可選欄位
z.string().optional();

// 下拉選單（非 null）
z.string().min(1, { error: "請選擇一個選項" });

// 整個表單 schema
const formSchema = z.object({
    username: z.string().trim().min(1, { error: "帳號為必填" }),
    email: z.string().email({ error: "請輸入有效的 Email" }),
    password: z.string().min(6, { error: "密碼至少 6 個字元" }),
});
</script>
```

## Zod 自訂錯誤訊息（Zod v4）

Zod v4 引入統一的 `error` 參數取代 v3 的多種 API（`message`、`errorMap`、`invalid_type_error`、`required_error`）。`message` 仍向下相容，但新程式碼建議使用 `error`。

### 錯誤優先順序（高 → 低）

| 優先順序  | 層級        | 說明                                         |
| --------- | ----------- | -------------------------------------------- |
| 1（最高） | Schema 層級 | 直接寫在 schema 定義或方法上的 `error`       |
| 2         | Parse 層級  | 傳入 `.parse()` / `.safeParse()` 的 `error`  |
| 3（最低） | 全域        | `z.config({ customError })` 設定的全域 error |

### 方法層級：字串直接傳入

最常用的方式，直接在驗證方法上寫 `error` 字串：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

✅ Zod v4 推薦寫法（error）
z.string().min(1, { error: "帳號為必填" });
</script>
```

### 方法層級：函式動態生成

當需要根據驗證結果動態產生錯誤訊息時，用函式：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

z.string().min(5, {
    error: (issue) => `至少需要 ${issue.minimum} 個字元，目前太短了`,
});
</script>
```

### Schema 層級：型別錯誤

直接在 schema 上設定，處理型別不符（如 undefined、null 傳入）的情境：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

// 字串直接傳入
z.string({ error: "此欄位必須是文字" });

// 函式：根據 input 值動態決定訊息
z.string({
    error: (issue) =>
        issue.input === undefined ? "此欄位為必填" : "此欄位必須是文字",
});

// 數字型別
z.number({
    error: (issue) =>
        issue.input === undefined ? "此欄位為必填" : "此欄位必須是數字",
});
</script>
```

### 全域錯誤設定

透過 `z.config()` 設定全域預設錯誤訊息，優先順序最低，適合做 fallback：

```javascript
// src/main.js
z.config({
    customError: (issue) => {
        if (issue.code === "invalid_type") {
            return `預期 ${issue.expected}，但收到 ${typeof issue.input}`;
        }
        if (issue.code === "too_small") {
            return `最小值為 ${issue.minimum}`;
        }
        // 回傳 undefined 則使用 Zod 內建訊息（zhTW locale）
        return undefined;
    },
});
```

### i18n 設定

```javascript
// src/main.js
z.config(z.locales.zhTW());
```

## Zod 自訂驗證邏輯（refine / superRefine）

當 Zod 內建的 `.min()`、`.email()`、`.regex()` 等不夠用時，用 `.refine()` 和 `.superRefine()` 撰寫自訂驗證邏輯。

### 使用場景

| 方法             | 使用時機                                        |
| ---------------- | ----------------------------------------------- |
| `.refine()`      | 單一條件驗證，回傳 `true`/`false`               |
| `.superRefine()` | 需要一次產生**多筆**錯誤，或需要指定 error code |
| 物件 `.refine()` | **跨欄位**驗證（如密碼確認、日期區間比對）      |

#### `.refine()` — 單一自訂驗證

接收一個 predicate 函式，回傳 `false` 時觸發錯誤：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

// 基本用法
z.string().refine((val) => val.includes("@"), {
    error: "必須包含 @ 符號",
});

// 動態錯誤訊息
z.string().refine((val) => val.length >= 8, {
    error: (issue) => `至少需要 8 個字元，目前只有 ${issue.input.length} 個`,
});
</script>
```

### `.superRefine()` — 多筆錯誤

當一個欄位需要一次回報多個問題時（例如密碼強度），用 `.superRefine()` 搭配 `ctx.addIssue()`：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

const strongPassword = z.string().superRefine((val, ctx) => {
    if (val.length < 8) {
        ctx.addIssue({
            code: "too_small",
            minimum: 8,
            origin: "string",
            inclusive: true,
            message: "密碼至少 8 個字元",
            input: val,
        });
    }
    if (!/[A-Z]/.test(val)) {
        ctx.addIssue({
            code: "custom",
            message: "需包含大寫字母",
            input: val,
        });
    }
    if (!/[a-z]/.test(val)) {
        ctx.addIssue({
            code: "custom",
            message: "需包含小寫字母",
            input: val,
        });
    }
    if (!/\d/.test(val)) {
        ctx.addIssue({
            code: "custom",
            message: "需包含數字",
            input: val,
        });
    }
});
</script>
```

**注意**：

1. `.refine()` 鏈式呼叫 vs `.superRefine()`
2. `.refine()` 鏈式呼叫第一個失敗就**停止**，後面的不會執行
3. `.superRefine()` 會**一次跑完**所有檢查，回報全部錯誤

### 物件層級 `.refine()` — 跨欄位驗證

驗證需要比對多個欄位時（如密碼確認），在 `z.object()` 後面加 `.refine()`：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

const schema = z
    .object({
        password: z.string().min(8, { error: "密碼至少 8 個字元" }),
        confirmPassword: z.string().min(1, { error: "請再次輸入密碼" }),
    })
    .refine((data) => data.password === data.confirmPassword, {
        error: "兩次密碼不一致",
        path: ["confirmPassword"], // 錯誤掛在 confirmPassword 欄位上
    });
</script>
```

**注意**：
`path` 參數指定錯誤要掛在哪個欄位，這樣 PrimeVue Form 的 `$form.confirmPassword?.invalid` 才會正確觸發。

### 非同步驗證（Async Refine）

需要呼叫 API 驗證時（如帳號是否已存在），用 async 函式：

```javascript
const schema = z.object({
    username: z
        .string()
        .min(1, { error: "帳號為必填" })
        .refine(
            async (val) => {
                const response = await api.get(`/users/check?username=${val}`);
                return response.data.available;
            },
            { error: "此帳號已被使用" },
        ),
});
```

**注意**

使用 async refine 時，Zod 需要用 `.parseAsync()` 或 `.safeParseAsync()` 來解析。確認 `zodResolver` 有支援非同步驗證再使用。

### 自訂驗證工具函式範例

將常用的自訂驗證抽成工具函式，方便跨表單重用：

```javascript
// utils/validators.js

/** 台灣身分證字號 */
export function isValidTaiwanId(id) {
    if (!/^[A-Z][12890]\d{8}$/i.test(id)) return false;
    const upper = id.toUpperCase();
    const map = {
        A: 10,
        B: 11,
        C: 12,
        D: 13,
        E: 14,
        F: 15,
        G: 16,
        H: 17,
        I: 34,
        J: 18,
        K: 19,
        L: 20,
        M: 21,
        N: 22,
        O: 35,
        P: 23,
        Q: 24,
        R: 25,
        S: 26,
        T: 27,
        U: 28,
        V: 29,
        W: 32,
        X: 30,
        Y: 31,
        Z: 33,
    };
    const n = map[upper[0]];
    let sum = Math.floor(n / 10) + (n % 10) * 9;
    for (let i = 0; i < 8; i++)
        sum += parseInt(upper[i + 1]) * [8, 7, 6, 5, 4, 3, 2, 1][i];
    sum += parseInt(upper[9]);
    return sum % 10 === 0;
}
```

在 schema 中使用：

```vue
<script setup>
// src/views/Login.vue
import { z } from "zod";

import { isValidTaiwanId } from "@/utils/validators";

const schema = z.object({
    nationalId: z
        .string()
        .min(1, { error: "身分證字號為必填" })
        .refine(isValidTaiwanId, { error: "身分證字號格式不正確" }),
});
</script>
```
