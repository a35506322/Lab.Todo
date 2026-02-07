<script setup>
import { watch } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useNotificationStore } from '@/stores/useNotificationStore';

const toast = useToast();
const notificationStore = useNotificationStore();

// Tips: 主要因 axios 攔截器中，將全域錯誤的 toast 設定儲存至 notificationStore，因此需要在 App.vue 中監聽 notificationStore.toasts 的變化，並顯示所有待顯示的 toasts
watch(
  () => notificationStore.toasts.length,
  () => {
    if (notificationStore.toasts.length > 0) {
      notificationStore.toasts.forEach((toastConfig) => {
        toast.add(toastConfig);
      });
      notificationStore.clear();
    }
  },
  { immediate: true },
);
</script>

<template>
  <div>
    <Toast position="top-center" />
    <ConfirmDialog />
    <RouterView />
  </div>
</template>

<style scoped></style>
