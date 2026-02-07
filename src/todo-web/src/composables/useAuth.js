import { createCookies } from '@vueuse/integrations/useCookies';

const COOKIE_NAME = 'token';
const cookieApi = createCookies()([COOKIE_NAME], { doNotParse: true });

// 清除舊的 localStorage token（如果存在）
if (typeof window !== 'undefined' && localStorage.getItem('token')) {
  localStorage.removeItem('token');
}

/**
 * 取得 token（可在 setup 外呼叫，例如 axios interceptor）
 */
export function getToken() {
  return cookieApi.get(COOKIE_NAME);
}

/**
 * 設定 token 至 cookie
 * @param {string} token
 * @param {number} expiresInMinutes - API 回傳的 ExpiresIn（分鐘）
 */
export function setToken(token, expiresInMinutes) {
  cookieApi.set(COOKIE_NAME, token, {
    path: '/',
    maxAge: (expiresInMinutes ?? 60) * 60, // universal-cookie 的 maxAge 單位是秒
  });
}

/**
 * 移除 token、登出
 */
export function removeToken() {
  cookieApi.remove(COOKIE_NAME, { path: '/' });
}
