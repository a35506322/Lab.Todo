<script setup>
import { ref } from 'vue';

const props = defineProps({
  text: String,
  class: String,
  type: String,
});

const buttonRef = ref();
</script>

<template>
  <button
    ref="buttonRef"
    class="group bg-background border-primary relative w-auto cursor-pointer overflow-hidden rounded-sm border-2 p-3 px-6 text-center font-semibold"
    :class="[props.class]"
    :type="type"
  >
    <div class="flex items-center justify-center gap-2">
      <div
        class="bg-primary size-2 scale-100 rounded-lg transition-all duration-300 group-hover:scale-[100.8]"
      />
      <span
        class="inline-block whitespace-nowrap transition-all duration-300 group-hover:translate-x-12 group-hover:opacity-0"
      >
        <span class="text-primary font-semibold tracking-wide">{{ text }}</span>
      </span>
    </div>

    <div
      class="text-primary-foreground absolute top-0 z-10 flex size-full translate-x-12 items-center justify-center gap-2 opacity-0 transition-all duration-300 group-hover:-translate-x-5 group-hover:opacity-100"
    >
      <span class="font-semibold tracking-wide whitespace-nowrap text-white">{{ text }}</span>
      <i class="pi pi-arrow-right font-semibold text-white"></i>
    </div>
  </button>
</template>

<style></style>
