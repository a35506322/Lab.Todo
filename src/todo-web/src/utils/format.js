/**
 * 格式化日期為台灣格式 (YYYY/MM/DD)
 * @param {string} dateString - 日期字串
 * @returns {string} 格式化後的日期字串，若輸入為空則回傳 '-'
 */
export const formatTWDateTime = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  });
};
