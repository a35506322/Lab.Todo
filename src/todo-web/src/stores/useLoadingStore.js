import { ref, computed } from 'vue';
import { defineStore } from 'pinia';

export const useLoadingStore = defineStore('loading', () => {
  const loadingCount = ref(0);

  /**
   * 是否正在載入中
   */
  const isLoading = computed(() => loadingCount.value > 0);

  /**
   * 顯示 loading（計數器 +1）
   */
  const show = () => {
    loadingCount.value++;
  };

  /**
   * 隱藏 loading（計數器 -1，不小於 0）
   */
  const hide = () => {
    if (loadingCount.value > 0) {
      loadingCount.value--;
    }
  };

  /**
   * 重置 loading 計數器
   */
  const reset = () => {
    loadingCount.value = 0;
  };

  return { isLoading, show, hide, reset };
});
