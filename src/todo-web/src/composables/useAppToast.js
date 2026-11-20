import { useToast } from 'openvue/usetoast';

/**
 * 應用程式統一的 Toast composable
 */
export function useAppToast() {
  const toast = useToast();

  /**
   * 新增 toast 訊息
   * @param {Object} options - OpenVue Toast message 選項
   * @param {string} options.severity - 'success' | 'info' | 'warn' | 'error'
   * @param {string} options.summary - 標題
   * @param {string} options.detail - 詳細內容
   * @param {number} [options.life] - 顯示時間（毫秒），預設 5000，可覆蓋
   */
  function add(options) {
    toast.add({
      life: 5000,
      ...options,
    });
  }

  /**
   * 移除所有群組的訊息
   */
  function removeAllGroups() {
    toast.removeAllGroups();
  }

  /**
   * 移除指定群組的訊息
   * @param {string} group - 群組名稱
   */
  function removeGroup(group) {
    toast.removeGroup(group);
  }

  return {
    add,
    removeAllGroups,
    removeGroup,
  };
}
