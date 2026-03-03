<script setup>
import { computed } from 'vue';

const props = defineProps({
  duration: {
    type: Number,
    default: 10,
  },
  radiantWidth: {
    type: Number,
    default: 100,
  },
  class: String,
});

const styleVar = computed(() => {
  return {
    '--radiant-anim-duration': `${props.duration}s`,
    '--radiant-width': `${props.radiantWidth}px`,
  };
});
</script>

<template>
  <p
    :style="styleVar"
    class="radiant-animation mx-auto max-w-md bg-[var(--p-primary-color)] bg-gradient-to-r from-transparent via-[var(--p-primary-200)] via-50% to-transparent [background-size:var(--radiant-width)_100%] bg-clip-text [background-position:0_0] bg-no-repeat text-transparent [transition:background-position_1s_cubic-bezier(.6,.6,0,1)_infinite]"
    :class="[$props.class]"
  >
    <slot />
  </p>
</template>

<style scoped>
@keyframes radiant {
  0%,
  90%,
  100% {
    background-position: calc(-100% - var(--radiant-width)) 0;
  }
  30%,
  60% {
    background-position: calc(100% + var(--radiant-width)) 0;
  }
}

.radiant-animation {
  animation: radiant var(--radiant-anim-duration) infinite;
}
</style>
