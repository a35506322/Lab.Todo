---
name: pinia
description: 適用於編寫 Pinia 時最佳實踐。
---

# Pinia 開發規範

適用於編寫 Pinia 時最佳實踐。

## 使用場景

-   當使用 Pinia 時
-   當更改 src/stores 底下 store 檔案時

## Setup Stores

命名規則：`use[XXX]Store`，useCounterStore 、 useUserStore 、 useProductStore 等
檔案位置：`stores/[XXX]Store.js`，useCounterStore.js 、 useUserStore.js 、 useProductStore.js 等
風格：統一使用 Composition API

正確示範：

```js
// src/stores/useCounterStore.js
✅ Composition API
import { ref, computed } from "vue";
import { defineStore } from "pinia";

export const useCounterStore = defineStore("counter", () => {
    // state
    const count = ref(0);
    const name = ref("Eduardo");

    // getters
    const doubleCount = computed(() => count.value * 2);

    // actions
    const increment = () => {
        count.value++;
    };

    return { count, name, doubleCount, increment };
});
```

錯誤示範：

```js
❌ Option API
import { ref, computed } from "vue";
import { defineStore } from "pinia";

export const useNotificationStore = defineStore("notification", {
    state: () => ({
        toasts: [],
    }),
    actions: {
        /**
         * 新增 toast 訊息
         * @param {Object} payload - PrimeVue Toast message 選項
         * @param {string} payload.severity - 'success' | 'info' | 'warn' | 'error'
         * @param {string} payload.summary - 標題
         * @param {string} payload.detail - 詳細內容
         * @param {number} [payload.life] - 顯示時間（毫秒），預設 5000
         */
        add(payload) {
            this.toasts.push({
                life: 5000,
                ...payload,
            });
        },
        /**
         * 清空所有待顯示的 toasts
         */
        clear() {
            this.toasts = [];
        },
    },
});
```

## 使用 storeToRefs 進行解構

```vue
<script setup>
import { storeToRefs } from "pinia";
import { useCounterStore } from "@/stores/counter";

const store = useCounterStore();

// ❌ Breaks reactivity
const { name, doubleCount } = store;

// ✅ Preserves reactivity for state/getters
const { name, doubleCount } = storeToRefs(store);

// ✅ Actions can be destructured directly
const { increment } = store;
</script>
```
