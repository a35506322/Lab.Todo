import axiosInstance from './axios-instance';

/**
 * 登入
 * @param {{ userId: string, password: string }} payload
 */
export function login(payload) {
  return axiosInstance.post('/user/login', {
    userId: payload.userId,
    password: payload.password,
  });
}
