import axios from 'axios';
import router from '@/router';
import { getToken, removeToken } from '@/composables/useAuth';
import { useNotificationStore } from '@/stores/useNotificationStore';

const baseURL = import.meta.env.VITE_API_BASE_URL;
const globalErrorToastMap = {
  401: {
    severity: 'error',
    summary: '登入失敗',
    detail: '帳號或密碼不正確，或者登入已過期',
  },
  403: {
    severity: 'error',
    summary: '權限不足',
    detail: '您沒有權限執行此操作',
  },
  422: {
    severity: 'error',
    summary: '商業邏輯錯誤',
    detail: '請檢查操作是否符合規則',
  },
  500: {
    severity: 'error',
    summary: '伺服器錯誤',
    detail: '系統發生異常，請稍後再試',
  },
};

const axiosInstance = axios.create({
  baseURL,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('Axios interceptors error:', error);
    const status = error.response?.status;
    const notificationStore = useNotificationStore();

    if (globalErrorToastMap[status]) {
      const toastConfig = { ...globalErrorToastMap[status] };
      if (error.response?.data?.message) {
        toastConfig.detail = error.response.data.message;
      }
      notificationStore.add(toastConfig);
    }

    if (status === 401) {
      removeToken();
      if (router.currentRoute.value.name !== 'login') {
        router.push({ name: 'login' });
      }
    } else if (status === 403) {
      router.push({ name: 'accessDenied' });
    } else if (status === 500) {
      router.push({ name: 'error' });
    }

    return Promise.reject(error);
  },
);

export default axiosInstance;
