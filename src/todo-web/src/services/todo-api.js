import axiosInstance from './axios-instance';

const TODO_BASE = '/todo';

/**
 * 查詢待辦事項清單
 * @param {{ todoTitle?: string, isComplete?: string, addUserId?: string }} params
 */
export const getTodos = (params) =>
  axiosInstance.get(`${TODO_BASE}/getTodoByQueryString`, { params });

/**
 * 查詢單筆待辦事項
 * @param {number} id
 */
export const getTodoById = (id) => axiosInstance.get(`${TODO_BASE}/getTodoById/${id}`);

/**
 * 新增待辦事項
 * @param {{ todoTitle: string, todoContent?: string }} payload
 */
export const createTodo = (payload) => axiosInstance.post(`${TODO_BASE}/insertTodo`, payload);

/**
 * 更新待辦事項
 * @param {number} id
 * @param {{ todoTitle: string, todoContent?: string, isComplete: string }} payload
 */
export const updateTodo = (id, payload) =>
  axiosInstance.put(`${TODO_BASE}/updateTodoById/${id}`, payload);

/**
 * 刪除待辦事項
 * @param {number} id
 */
export const deleteTodo = (id) => axiosInstance.delete(`${TODO_BASE}/deleteTodoById/${id}`);
