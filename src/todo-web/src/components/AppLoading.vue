<script setup>
import { storeToRefs } from 'pinia';
import { useLoadingStore } from '@/stores/useLoadingStore';
import BlockUI from 'primevue/blockui';
import ProgressSpinner from 'primevue/progressspinner';

const loadingStore = useLoadingStore();
const { isLoading } = storeToRefs(loadingStore);
</script>

<template>
  <BlockUI :blocked="isLoading" full-screen />
  <Teleport to="body">
    <div
      v-if="isLoading"
      class="fixed inset-0 flex flex-col items-center justify-center gap-3"
      style="z-index: 9999"
    >
      <ProgressSpinner
        style="width: 50px; height: 50px"
        stroke-width="6"
        aria-label="載入中"
      />
    </div>
  </Teleport>
</template>
