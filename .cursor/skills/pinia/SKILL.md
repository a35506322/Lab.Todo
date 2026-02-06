---
name: pinia
description: 適用於編寫 Pinia 時最佳實踐。
---

# Pinia 開發規範

適用於編寫 Pinia 時最佳實踐。

## Setup Stores

命名規則：`use[XXX]Store`，useCounterStore 、 useUserStore 、 useProductStore 等

檔案位置：`stores/[XXX]Store.js`，useCounterStore.js 、 useUserStore.js 、 useProductStore.js 等

```js
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
