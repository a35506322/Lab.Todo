---
name: primevue
description: 適用於編寫 PrimeVue 時最佳實踐，包含 PrimeVue 使用方式、Form 表單驗證等。
---

# PrimeVue 開發規範

使用 PrimeVue 作為 Vue 3 的 UI 元件庫，提供了一系列高品質的 UI 元件，適用於各種前端專案。

## 使用場景

- 當使用 PrimeVue 時
- 當需要使用 PrimeVue 的 Form 表單驗證時

## PrimeVue 使用方式

使用 `mcp primevue` 查詢 PrimeVue 的用法。

```
"What are all the props available for the DataTable component?"
"Show me how to implement row selection in DataTable"
"How do I customize Button styles using Pass Through?"
"What design tokens are available for the Card component?"
"Find me a component for selecting multiple items from a list"
"Compare AutoComplete and Select components"
```

## Form 表單驗證

PrimeVue Forms (`@primevue/forms`) 提供表單狀態管理與驗證功能。本專案統一使用 [zod](../zod/SKILL.md) 作為驗證 schema。

### 核心概念

Form 元件有三個必要屬性：

| 屬性            | 說明                                                  |
| --------------- | ----------------------------------------------------- |
| `initialValues` | 表單初始值物件，key 對應欄位 `name`                   |
| `resolver`      | 驗證器，回傳 `{ errors }` 物件                        |
| `@submit`       | 提交回呼，參數包含 `{ valid, values, errors, reset }` |

表單內的 PrimeVue 元件透過 `name` 屬性（非 `v-model`）綁定到表單狀態。

### 基礎範例

```vue
<script setup>
import { reactive } from "vue";
import { useToast } from "primevue/usetoast";
import { zodResolver } from "@primevue/forms/resolvers/zod";
import { z } from "zod";

const toast = useToast();

// 1. 定義初始值
const initialValues = reactive({
    username: "",
});

// 2. 定義 Zod schema + resolver
const resolver = zodResolver(
    z.object({
        username: z.string().trim().min(1, { message: "帳號為必填" }),
    }),
);

// 3. 處理提交
const onFormSubmit = async ({ valid, values }) => {
    if (!valid) return;

    // values.username, values.password 已通過驗證
    console.log("表單值:", values);
};
</script>

<template>
    <Form
        v-slot="$form"
        :initial-values="initialValues"
        :resolver="resolver"
        @submit="onFormSubmit"
    >
        <!-- 欄位區塊：輸入框 + 錯誤訊息 -->
        <div class="flex flex-col gap-1">
            <InputText
                name="username"
                type="text"
                placeholder="帳號"
                fluid
                :invalid="$form.username?.invalid"
            />
            <Message
                v-if="$form.username?.invalid"
                severity="error"
                size="small"
                variant="simple"
            >
                {{ $form.username?.error?.message }}
            </Message>
        </div>
        <Button type="submit" label="送出" />
    </Form>
</template>
```

### 欄位錯誤訊息顯示模式

**單一錯誤訊息**（取第一筆）：

```vue
<Message
    v-if="$form.username?.invalid"
    severity="error"
    size="small"
    variant="simple"
>
  {{ $form.username?.error?.message }}
</Message>
```

**多筆錯誤訊息**（例如密碼規則）：

```vue
<Message
    v-if="$form.password?.invalid"
    severity="error"
    size="small"
    variant="simple"
>
  <ul class="my-0 px-4 flex flex-col gap-1">
    <li v-for="(error, index) of $form.password.errors" :key="index">
      {{ error.message }}
    </li>
  </ul>
</Message>
```

### 驗證觸發時機 (validateOn)

Form 預設在 submit 時驗證，可透過以下 props 調整：

| Prop                    | 說明               | 預設值      |
| ----------------------- | ------------------ | ----------- |
| `validateOnValueUpdate` | 值變更時即時驗證   | `true`      |
| `validateOnBlur`        | 失焦時驗證         | `false`     |
| `validateOnMount`       | 掛載時驗證指定欄位 | `undefined` |
| `validateOnSubmit`      | 提交時驗證         | `true`      |

```vue
<!-- 表單級別：關閉即時驗證，改為失焦驗證 -->
<Form
    :initial-values="initialValues"
    :resolver="resolver"
    :validate-on-value-update="false"
    :validate-on-blur="true"
    @submit="onFormSubmit"
>
  <!-- 欄位級別：可覆蓋表單設定 -->
  <InputText
    name="username"
    :form-control="{ validateOnValueUpdate: true }"
  />
</Form>
```

### Submit 事件參數

`@submit` 回呼的事件物件包含以下屬性：

| 屬性            | 型別       | 說明                                 |
| --------------- | ---------- | ------------------------------------ |
| `originalEvent` | `Event`    | 原生 form submit 事件                |
| `valid`         | `boolean`  | 表單是否通過驗證                     |
| `values`        | `object`   | 所有欄位的值                         |
| `states`        | `object`   | 各欄位的狀態（含 invalid、dirty 等） |
| `errors`        | `object`   | 驗證錯誤物件                         |
| `reset`         | `function` | 重置表單至初始狀態                   |

```javascript
const onFormSubmit = async ({ valid, values, reset }) => {
    if (!valid) return;

    try {
        await api.post("/data", values);
        reset(); // 提交成功後重置表單
    } catch (error) {
        // 處理錯誤
    }
};
```

### 自訂 Resolver（不使用 Zod）

如果不想用 Zod，可以寫自訂 resolver。resolver 接收 `{ values }` 回傳 `{ errors }`：

```javascript
const resolver = ({ values }) => {
    const errors = {};

    if (!values.username) {
        errors.username = [{ message: "帳號為必填" }];
    }

    if (!values.password) {
        errors.password = [{ message: "密碼為必填" }];
    } else if (values.password.length < 6) {
        errors.password = [{ message: "密碼至少 6 個字元" }];
    }

    return { errors };
};
```

### 使用 Zod 進行驗證

文件使用 [zod](../zod/SKILL.md) 進行驗證。

### invalid 屬性綁定

PrimeVue 表單元件支援 `:invalid` prop 來顯示錯誤狀態的紅色邊框：

```vue
<InputText name="username" :invalid="$form.username?.invalid" />

<Password name="password" :invalid="$form.password?.invalid" />

<Select name="category" :invalid="$form.category?.invalid" />
```

### 注意事項

1. **Form 元件已全域註冊**，不需要 import，直接在 template 使用 `<Form>`
2. **使用 `name` 而非 `v-model`**：Form 內的元件透過 `name` 綁定，不要同時使用 `v-model`
3. **Zod 已設定 zhTW locale**：`z.config(z.locales.zhTW())`，使用 Zod 內建規則時會自動顯示中文訊息，自訂 `error`（或 `message`）會覆蓋內建中文。Zod v4 建議用 `error` 取代 `message`
4. **`initialValues` 使用 `reactive`**：確保表單初始值是響應式的
5. **resolver 是靜態的**：`zodResolver(schema)` 在宣告時就會建立，不會每次驗證重新建立
