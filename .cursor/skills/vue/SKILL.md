---
name: vue
description: 適用於編寫 Vue SFC 撰寫 Vue 3 Composition API,、script setup、reactivity system、component 時最佳實踐。
---

# Vue 開發規範

適用於編寫 Vue SFC 撰寫 Vue 3 Composition API,、script setup、reactivity system、component 時最佳實踐。

## 使用場景

-   當使用 Vue 3 Composition API 時
-   當使用 script setup 時
-   當使用 reactivity system 時
-   當使用 component 時
-   當使用 composable 時 （VueUse）
-   當使用 Vue 的 lifecycle hooks 時

## Script Setup

`<script setup>` 是採用組合式 API 的 Vue 單文件組件推薦的語法。此語法能提供更佳的執行時效能與 IDE 類型推斷功能

```vue
<!-- HomeView.vue - Single-File Component -->
<script setup>
import { ref } from "vue";

const title = ref("Hello");

const handleClick = () => {
    // ...
};
</script>

<template>
    <div class="container">
        <h1>{{ title }}</h1>
        <button @click="handleClick">Click me</button>
    </div>
</template>

<style scoped>
/* Styles only apply to this component */
.container {
    padding: 1rem;
}

h1 {
    color: #42b883;
}
</style>
```

## ref 與 shallowRef

-   多數情況使用 `ref`

```vue
<script setup>
import { ref } from "vue";

// ref - deep reactivity (tracks nested changes)
const user = ref({ name: "John", profile: { age: 30 } });
user.value.profile.age = 31; // Triggers reactivity
</script>

<template>
    <!-- `ref` 在 template 中不需要使用 `.value`，Vue 會自動展開 -->
    <p>{{ user.name }}</p>
</template>
```

-   大量且不會被修改的資料使用 `shallowRef`

```vue
<script setup>
import { shallowRef } from "vue";

✅ Large paginated data - only care when page changes
const pageData = shallowRef([]);

const loadPage = async (page) => {
    ✅ Replace entirely to trigger reactivity
    pageData.value = await api.getPage(page);
};
</script>

<template>
    <div v-for="item in pageData" :key="item.id">
        {{ item.name }}
    </div>
</template>
```

## computed

-   基本示範

```vue
<script setup>
import { ref, computed } from "vue";

const count = ref(0);

// Read-only computed
const doubled = computed(() => count.value * 2);

// Writable computed
const plusOne = computed({
    get: () => count.value + 1,
    set: (val) => {
        count.value = val - 1;
    },
});
</script>
```

-   樣式結合示範

```vue
<script setup>
import { computed } from "vue";

const props = defineProps({
    color: String,
    fontSize: Number,
    isHighlighted: Boolean,
});

const textStyles = computed(() => ({
    color: props.color,
    fontSize: `${props.fontSize}px`,
    backgroundColor: props.isHighlighted ? "yellow" : "transparent",
    fontWeight: props.isHighlighted ? "bold" : "normal",
}));
</script>

<template>
    <span :style="textStyles">Styled text</span>
</template>
```

-   先讀依賴、再分支，避免首次執行時漏追蹤

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

-   會改變原陣列的方法（sort、reverse 等）在 computed 內先複製再呼叫

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

-   非用於執行操作或修改狀態

```vue
<script setup>
import { ref, computed } from "vue";

const items = ref([]);
const count = ref(0);
const lastFetch = ref(null);

❌ Mutates other state
const doubledCount = computed(() => {
    count.value++; // Side effect - modifying state!
    return count.value * 2;
});

✅ Pure computation only
const doubledCount = computed(() => {
  return count.value * 2
})

❌ Makes async request
const userData = computed(async () => {
    const response = await fetch("/api/user"); // Side effect - API call!
    return response.json();
});

✅ Use lifecycle hook for initial fetch
onMounted(async () => {
  const response = await fetch('/api/user')
  userData.value = await response.json()
})

✅ Pure filtering
const highlightedItems = computed(() => {
  return items.value.filter(i => i.highlighted)
})

❌ Modifies DOM
const highlightedItems = computed(() => {
    document.title = `${items.value.length} items`; // Side effect - DOM mutation!
    return items.value.filter((i) => i.highlighted);
});

❌ Writes to external state
const processedData = computed(() => {
    lastFetch.value = new Date(); // Side effect - modifying state!
    return items.value.map((i) => i.name);
});


✅ Use watcher for side effects
watch(items, (newItems) => {
  document.title = `${newItems.length} items`
}, { immediate: true })

// Increment count through event handler, not computed
function increment() {
  count.value++
}
</script>
```

## Watch

-   基本示範

```vue
<script setup>
import { ref, watch } from "vue";

const count = ref(0);

✅ Watch single ref
watch(count, (newVal, oldVal) => {
    console.log(`Changed from ${oldVal} to ${newVal}`);
});

const state = ref({ count: 0, name: "Vue" });

✅ Watch getter
watch(
    () => state.count,
    (newCount, oldCount) => {
        console.log(`Count changed from ${oldCount} to ${newCount}`);
    }
);

const state = reactive({ firstName: "John", lastName: "Doe" });

✅ Watch multiple sources
watch(
    [() => state.firstName, () => state.lastName],
    ([newFirst, newLast], [oldFirst, oldLast]) => {
        console.log(
            `Name changed from ${oldFirst} ${oldLast} to ${newFirst} ${newLast}`
        );
    }
);
</script>
```

-   不要 watch 整包資料，而是監聽特定屬性

```vue
<script setup>
import { ref, watch } from "vue";

const state = ref({
    users: [],,
});

❌ Watch entire object
watch(state, (newState) => {
  console.log('State changed:', newState)
})

✅ Watch specific property
watch(
  () => state.users.filter(u => u.active).length,
  (userId) => {
    loadUserDetails(userId)
  }
)
</script>
```

-   立即執行監聽

```vue
<script setup>
import { ref, watch } from "vue";

const userId = ref(1);
const userData = ref(null);

// GOOD: Single watch with immediate runs on mount and on change
watch(
    userId,
    async (newId) => {
        const response = await fetch(`/api/users/${newId}`);
        userData.value = await response.json();
    },
    { immediate: true }
);
</script>
```

## Lifecycle Hooks

多數情況下，只使用 `onMounted`

```vue
<script setup>
import { onMounted } from "vue";

onMounted(() => {
    console.log("DOM is ready");
});
</script>
```

## Composables

‼️ Composables 一律使用 `VueUse` 套件，可使用 `mcp context7` 查詢`VueUse`套件，除非真的沒有才使用自定義 composable

命名規則：`use[XXX]`，useMouse 、 useFetch 、 useCounter 等

-   基本示範

```vue
<script setup>
// composables/useMouse.ts
import { ref, onMounted, onUnmounted } from "vue";

export const useMouse = () => {
    const x = ref(0);
    const y = ref(0);

    const update = (e: MouseEvent) => {
        x.value = e.pageX;
        y.value = e.pageY;
    };

    onMounted(() => window.addEventListener("mousemove", update));
    onUnmounted(() => window.removeEventListener("mousemove", update));

    return { x, y };
};
</script>
```

## Component

－　命名規則：PascalCase，[XXX]t.vue（如 MyButton.vue）

-   基本示範

````vue
<script setup>
// components/UserComponent.vue
import { computed, watch, onMounted } from "vue";

const props = defineProps({
    title: String,
    count: Number,
});
const emit = defineEmits(["update"]);
const model = defineModel({
    type: String,
    required: true,
});
const doubled = computed(() => (props.count ?? 0) * 2);

watch(
    () => props.title,
    (newVal) => {
        console.log("Title changed:", newVal);
    }
);

onMounted(() => {
    console.log("Component mounted");
});

-   Props 單向傳遞
```vue
<script setup>
import { defineProps } from "vue";

const props = defineProps({
    title: String,
});

❌ Props 單向傳遞不應該被修改
const updatedTitle = () => {
    props.title = props.title.toUpperCase();
};

✅ 1. 使用 emit 發出事件
const updateTitle = () => {
   emit("update:title", props.title.toUpperCase());
};

✅ 2. 複製 props 的值，當父元件修改時同步修改
const localValue = ref(props.title)
watch(() => props.title, (newVal) => {
  localValue.value = newVal
}, { immediate: true })

✅ 3. 使用 computed 計算值
const updatedTitle = computed(() => {
  return props.title.toUpperCase();
});
</script>
````

-   props 傳遞始終保持響應式

```vue
<script setup>
import { computed } from "vue";

const props = defineProps({
    userId: Number,
    searchQuery: String,
});

✅ Use computed to create reactive URL
const userUrl = computed(() => `/api/users/${props.userId}`);
</script>
```

```vue
<script setup>
import { toRefs } from "vue";
import { useUserForm } from "./composables/useUserForm";

const props = defineProps({
    initialName: String,
    initialEmail: String,
    initialAge: Number,
});

✅ Convert all props to refs, preserving reactivity
const { initialName, initialEmail, initialAge } = toRefs(props);

// Now each is a ref that tracks prop changes
const { form, isValid } = useUserForm({
    name: initialName,
    email: initialEmail,
    age: initialAge,
});
</script>
```

## 效能

-   避免傳遞所有子元素都用來比較的父級狀態

錯誤示範：

```vue
<template>
    ❌ activeId changes -> ALL 100 ListItems re-render
    <ListItem
        v-for="item in list"
        :key="item.id"
        :id="item.id"
        :active-id="activeId"
    />
</template>

<script setup>
import { ref } from "vue";

const list = ref([
    /* 100 items */
]);
const activeId = ref(null);

// When activeId changes from 1 to 2:
// - ListItem 1 needs to re-render (was active, now not)
// - ListItem 2 needs to re-render (was not active, now active)
// - All other 98 ListItems ALSO re-render because activeId prop changed!
</script>
```

```vue
<!-- ListItem.vue - receives activeId and compares internally -->
<template>
    <div :class="{ active: id === activeId }">
        {{ id }}
    </div>
</template>

<script setup>
defineProps({
    id: Number,
    activeId: Number, // This prop changes for ALL items
});
</script>
```

正確示範：

```vue
<template>
    ✅ Only items whose :active actually changed will re-render
    <ListItem
        v-for="item in list"
        :key="item.id"
        :id="item.id"
        :active="item.id === activeId"
    />
</template>

<script setup>
import { ref } from "vue";

const list = ref([
    /* 100 items */
]);
const activeId = ref(null);

// When activeId changes from 1 to 2:
// - ListItem 1: :active changed from true to false -> re-renders
// - ListItem 2: :active changed from false to true -> re-renders
// - All other 98 ListItems: :active is still false -> NO re-render!
</script>
```

```vue
<!-- ListItem.vue - receives pre-computed boolean -->
<template>
    <div :class="{ active }">
        {{ id }}
    </div>
</template>

<script setup>
defineProps({
    id: Number,
    active: Boolean, // This only changes for items that truly changed
});
</script>
```

-   使用 v-once 跟 v-memo 避免不必要的重新渲染

```vue
<template>
    ✅ v-once tells Vue this never needs to update
    <footer v-once>
        <p>Copyright {{ copyrightYear }} {{ companyName }}</p>
    </footer>
</template>

<script setup>
const copyrightYear = 2024;
const companyName = "Acme Corp";
</script>
```

```vue
<template>
    ✅ Items only re-render when their selection state changes
    <div v-for="item in list" :key="item.id" v-memo="[item.id === selectedId]">
        <div :class="{ selected: item.id === selectedId }">
            <ExpensiveComponent :data="item" />
        </div>
    </div>
</template>

<script setup>
import { ref } from "vue";

const list = ref([
    /* many items */
]);
const selectedId = ref(null);

// When selectedId changes:
// - Only the previously-selected item re-renders (selected: true -> false)
// - Only the newly-selected item re-renders (selected: false -> true)
// - All other items are SKIPPED (v-memo values unchanged)
</script>
```

## 常見 css 撰寫

-   靜態與動態樣式的結合

```vue
<template>
    <!-- Static styles + dynamic styles are merged -->
    <div
        style="border: 1px solid gray"
        :style="{ backgroundColor: dynamicColor }"
    >
        Content
    </div>
</template>
```

-   合併多個樣式

```vue
<script setup>
const baseStyles = {
    padding: "10px",
    borderRadius: "4px",
};

const themeStyles = computed(() => ({
    backgroundColor: isDark.value ? "#333" : "#fff",
    color: isDark.value ? "#fff" : "#333",
}));
</script>

<template>
    <!-- Later objects override earlier ones for conflicting properties -->
    <div :style="[baseStyles, themeStyles]">Content</div>
</template>
```

-   Tailwind 動態產生 class 名稱

```vue
<script setup>
const props = defineProps({
    color: String,
    variant: String, // 'primary', 'secondary', 'danger'
});

// Use a mapping object with complete class names
const colorClasses = {
    red: "bg-red-500",
    blue: "bg-blue-500",
    green: "bg-green-500",
};
</script>

<template>
    <!-- CORRECT: Full class names that Tailwind can detect -->
    <div :class="[colorClasses[color]]">Content</div>
    <!-- CORRECT: All class names are complete strings -->
    <button
        :class="{
            'bg-blue-500 hover:bg-blue-600': variant === 'primary',
            'bg-gray-500 hover:bg-gray-600': variant === 'secondary',
        }"
    >
        Click me
    </button>
</template>
```

## 常見錯誤

-   在 scoped css 中使用 :deep() 來選擇 v-html 的內容

```vue
<script setup>
import { ref } from "vue";

const htmlContent = ref('<p class="dynamic">This is dynamic content</p>');
</script>

<template>
    <div class="container">
        <div v-html="htmlContent"></div>
    </div>
</template>

<style scoped>
/* GOOD: Use :deep() for v-html content */
.container :deep(.dynamic) {
    color: red;
    font-weight: bold;
}
</style>
```
