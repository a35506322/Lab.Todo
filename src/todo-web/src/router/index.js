import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import AppLayout from '@/layout/AppLayout.vue';
import { getToken } from '@/composables/useAuth';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: AppLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '/',
          name: 'home',
          component: HomeView,
        },
        {
          path: '/todo',
          name: 'todo',
          component: () => import('@/views/todo/Todo.vue'),
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'notfound',
      meta: { requiresAuth: false },
      component: () => import('@/views/NotFound.vue'),
    },
    {
      path: '/login',
      name: 'login',
      meta: { requiresAuth: false },
      component: () => import('@/views/Login.vue'),
    },
    {
      path: '/access',
      name: 'accessDenied',
      meta: { requiresAuth: false },
      component: () => import('@/views/Access.vue'),
    },
    {
      path: '/error',
      name: 'error',
      meta: { requiresAuth: false },
      component: () => import('@/views/Error.vue'),
    },
  ],
});

router.beforeEach((to) => {
  const hasToken = !!getToken();
  const requiresAuth = to.meta.requiresAuth !== false; // 預設需要 auth

  // 需要認證但沒有 token，導向登入頁並記錄 redirect
  // redirect 是為了在登入成功後，將使用者導回原本想訪問的頁面
  if (requiresAuth && !hasToken) {
    return {
      name: 'login',
      query: { redirect: to.fullPath },
    };
  }

  // 已登入但訪問登入頁，導向首頁或 redirect
  if (to.name === 'login' && hasToken) {
    const redirect = to.query.redirect;
    return redirect && typeof redirect === 'string' ? redirect : '/';
  }
});

export default router;
