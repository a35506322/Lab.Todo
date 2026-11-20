import axiosInstance from './axios-instance';

const TODO_BASE = '/todo';

/**
 * 查詢待辦事項清單
 * @param {{ todoTitle?: string, isComplete?: string, addUserId?: string }} params
 */
export function getTodos(params) {
  return axiosInstance.get(`${TODO_BASE}/getTodoByQueryString`, { params });
}

/**
 * 查詢單筆待辦事項
 * @param {number} id
 */
export function getTodoById(id) {
  return axiosInstance.get(`${TODO_BASE}/getTodoById/${id}`);
}

/**
 * 新增待辦事項
 * @param {{ todoTitle: string, todoContent?: string }} payload
 */
export function createTodo(payload) {
  return axiosInstance.post(`${TODO_BASE}/insertTodo`, payload);
}

/**
 * 更新待辦事項
 * @param {number} id
 * @param {{ todoTitle: string, todoContent?: string, isComplete: string }} payload
 */
export function updateTodo(id, payload) {
  return axiosInstance.put(`${TODO_BASE}/updateTodoById/${id}`, payload);
}

/**
 * 刪除待辦事項
 * @param {number} id
 */
export function deleteTodo(id) {
  return axiosInstance.delete(`${TODO_BASE}/deleteTodoById/${id}`);
}
