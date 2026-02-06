import axios from 'axios';
import router from '@/router';

const baseURL = import.meta.env.VITE_API_BASE_URL;

const axiosInstance = axios.create({
  baseURL,
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      router.push('/login');
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
