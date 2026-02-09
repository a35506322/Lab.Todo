---
name: vue-router
description:當使用 Vue Router 時運用此技能，裡面包含 Vue Router 最佳實踐規則、Navigation Guard 最佳實踐規則、Error Handling 最佳實踐規則等
---

# Vue Router

當使用 Vue Router 時運用此技能，裡面包含 Vue Router 最佳實踐規則、Navigation Guard 最佳實踐規則、Error Handling 最佳實踐規則等

## 使用場景

-   當使用 Vue Router 時
-   當更改 src/router/index.js 時
-   當需要使用 Navigation Guard 時
-   當需要使用 Vue Router 的 Error Handling 時

## 基本範例

````vue
<script setup>
import { useRouter } from "vue-router";

## 最佳實踐

```js
// router/index.js
import { createRouter, createWebHistory } from "vue-router";
const baseUrl = import.meta.env.BASE_URL;

const routes = [
    {
        path: "/",
        name: "Home",
        component: () => import("@/views/Home.vue"), // Lazy loaded
        meta: { requiresAuth: false },
    },
    {
        path: "/dashboard",
        name: "Dashboard",
        component: () => import("@/views/Dashboard.vue"),
        meta: { requiresAuth: true },
        children: [
            {
                path: "settings",
                name: "Settings",
                component: () => import("@/views/Settings.vue"),
            },
        ],
    },
    {
        path: "/users/:id",
        name: "UserProfile",
        component: () => import("@/views/UserProfile.vue"),
        props: true, // Pass params as props
    },
    {
        path: "/:pathMatch(.*)*",
        name: "NotFound",
        component: () => import("@/views/NotFound.vue"),
    },
];

const router = createRouter({
    history: createWebHistory(baseUrl),
    routes,
    scrollBehavior(to, from, savedPosition) {
        return savedPosition || { top: 0 };
    },
});

// Global navigation guard
router.beforeEach((to, from) => {
    if (to.meta.requiresAuth && !isAuthenticated()) {
        return { name: "Login", query: { redirect: to.fullPath } };
    }
});

export default router;
````

```js
// main.js
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

createApp(App).use(router).mount("#app");
```

```vue
<!-- App.vue -->
<template>
    <nav>
        <router-link to="/">Home</router-link>
        <router-link to="/dashboard">Dashboard</router-link>
    </nav>

    <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
            <component :is="Component" />
        </transition>
    </router-view>
</template>
```

## Navigation Guard 正確使用 async 與 await

```js
✅ Async function with proper returns
router.beforeEach(async (to, from) => {
    if (to.meta.requiresAuth) {
        try {
            const isAuthenticated = await checkAuth();

            if (!isAuthenticated) {
                return { name: "Login", query: { redirect: to.fullPath } };
            }
        } catch (error) {
            console.error("Auth check failed:", error);
            return {
                name: "Error",
                params: { message: "Authentication failed" },
            };
        }
    }
    // Explicitly return nothing to proceed
    return true;
});
```

## 4.2 版本 Navigation Guard 禁止使用 next()

```js
// router/index.js
✅ Return-based syntax (modern Vue Router 4+)
router.beforeEach((to, from) => {
    if (!isAuthenticated) {
        return "/login"; // Redirect
    }
    // Return nothing (undefined) to proceed
});

✅ Return false to cancel navigation
router.beforeEach((to, from) => {
    if (hasUnsavedChanges) {
        return false; // Cancel navigation
    }
});

✅ Async with return-based syntax
router.beforeEach(async (to, from) => {
    const user = await fetchUser();
    if (!user) {
        return { name: "Login", query: { redirect: to.fullPath } };
    }
    // Proceed with navigation
});
```

## Navigation Guard Error Handling

```js
// router/index.js
router.beforeEach(async (to, from) => {
    try {
        await validateAccess(to);
        // Proceed
    } catch (error) {
        if (error.status === 401) {
            return "/login";
        }
        if (error.status === 403) {
            return "/forbidden";
        }
        // Log error and proceed anyway (or return false)
        console.error("Access validation failed:", error);
        return false;
    }
});
```

## 外部資源參考

如果有任何疑問可以參考以下資源：
正常來說使用 `mcp context7` 進行查詢最佳使用做法
但發生如果有些異常情況無法解決優顯使用以下資源

1. [Vue Router 最佳實踐規則](https://github.com/antfu/skills/blob/main/skills/vue-router-best-practices/SKILL.md)
