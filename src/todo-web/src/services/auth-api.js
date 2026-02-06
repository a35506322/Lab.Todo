import axiosInstance from './axios-instance';

/**
 * 登入
 * @param {{ UserId: string, Password: string }} payload
 */
export const login = (payload) =>
  axiosInstance.post('/user/login', {
    UserId: payload.UserId,
    Password: payload.Password,
  });
