---
name: vue
description: vue 最佳實踐處理原則
---

# Vue 最佳實踐

適用於編寫 Vue SFC 撰寫 Vue 3 Composition API,、script setup、reactivity system、component 時最佳實踐。

## shallowRef

對於`不會被修改`的大型 API 回應資料，請使用 `shallowRef` 代替 `ref`，如真需要更新請完整取代

```vue
<script setup>
import { shallowRef } from "vue";

// Large paginated data - only care when page changes
const pageData = shallowRef([]);

const loadPage = async (page) => {
    // Replace entirely to trigger reactivity
    pageData.value = await api.getPage(page);
};
</script>

<template>
    <div v-for="item in pageData" :key="item.id">
        {{ item.name }}
    </div>
</template>
`
```

## computed

條件邏輯導致某屬性在首次執行時未被存取

```vue
<script setup>
import { ref, computed } from 'vue'

const isEnabled = ref(false)
const data = ref('important data')

❌ If isEnabled is false initially, data.value is never accessed
// Vue won't track 'data' as a dependency!
const result = computed(() => {
  if (!isEnabled.value) {
    return 'disabled'
  }
  return data.value  // This dependency may not be tracked
})

 ✅ Access all dependencies first
const result = computed(() => {
  const enabled = isEnabled.value
  const currentData = data.value  // Always accessed

  if (!enabled) {
    return 'disabled'
  }
  return currentData
})
</script>
```

使用 `computed` 時，請使用 `spread operator` 建立副本，避免直接修改原始資料

當使用 js 的 reverse() 、 sort() 、 splice() 、 push() 、 pop() 、 shift() 及 unshift() 會變更原始陣列，而非產生新陣列，因此需要使用 spread operator 建立副本，避免直接修改原始資料

```vue
<script setup>
import { ref, computed } from "vue";

const books = ref(["Vue Guide", "React Handbook"]);

✅ Create a copy before mutating for display
const sortedBooks = computed(() => {
    return [...books.value].sort(); // Spread to create copy before sort
});

const reversedBooks = computed(() => {
    return [...books.value].reverse(); // Spread to create copy before reverse
});
</script>
```
