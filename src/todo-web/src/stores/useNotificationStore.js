import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useNotificationStore = defineStore('notification', () => {
  const toasts = ref([]);

  /**
   * 新增 toast 訊息（供 axios 攔截器等非元件情境使用，由 App.vue 監聽後轉交給 PrimeVue Toast）
   * @param {Object} payload - PrimeVue Toast message 選項
   * @param {string} payload.severity - 'success' | 'info' | 'warn' | 'error'
   * @param {string} payload.summary - 標題
   * @param {string} payload.detail - 詳細內容
   * @param {number} [payload.life] - 顯示時間（毫秒），預設 5000
   */
  const add = (payload) => {
    toasts.value.push({
      life: 5000,
      ...payload,
    });
  };

  /**
   * 清空所有待顯示的 toasts
   */
  const clear = () => {
    toasts.value = [];
  };

  return { toasts, add, clear };
});
