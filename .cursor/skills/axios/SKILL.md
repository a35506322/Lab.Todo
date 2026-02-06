---
name: axios
description: 適用於編寫 Axios 時最佳實踐。
---

# Axios 開發規範

前端串接 API 使用 Axios 進行串接，並將 API 封裝成函數，方便後續使用。

## 資料夾位置

```
services/
├── axios-instance.js
├── todo-api.js
```

## 封裝 Axios Instance

```js
import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL;

const axiosInstance = xios.create({
    baseURL: baseURL,
});

// Add a request interceptor
axiosInstance.interceptors.request.use(
    (config) => {
        // Add JWT token to the request header
        const token = localStorage.getItem("token");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        // Do something with the request error
        return Promise.reject(error);
    }
);

// Add a response interceptor
axiosInstance.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        // 根據 response.status 進行錯誤處理
        if (response.status === 401) {
            // .... 進行錯誤處理
            router.push("/login");
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;
```

## 封裝 API 函數

不要在封裝函數中使用任何邏輯只需單純呼叫 axiosInstance 並傳入相應的參數，邏輯應該是用戶端處理，
如有錯誤希望共用判斷請在 axiosInstance 的 response interceptor 中處理。

```js
// services/todo-api.js
import axiosInstance from "./axios-instance";

// 取得全部 Todo
export const getTodos = () => axiosInstance.get("/todos");

// 取得單一 Todo
export const getTodoById = (id) => axiosInstance.get(`/todos/${id}`);

// 新增 Todo
export const createTodo = (payload) => axiosInstance.post("/todos", payload);

// 更新 Todo
export const updateTodo = (id, payload) =>
    axiosInstance.put(`/todos/${id}`, payload);

// 刪除 Todo
export const deleteTodo = (id) => axiosInstance.delete(`/todos/${id}`);
```

## 使用方式

使用非同步 `try catch` 擷取 axios 錯誤

```vue
<script setup>
import { ref, onMounted } from "vue";
import { getTodos } from "./todo-api";

const todos = ref([]);
onMounted(async () => {
    try {
        const response = await getTodos();
        todos.value = response.data;
    } catch (error) {
        console.error(error);
        todos.value = [];
    }
});
</script>

<template>
    <div v-for="todo in todos" :key="todo.id">
        {{ todo.title }}
    </div>
</template>
```
