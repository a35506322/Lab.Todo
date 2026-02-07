<script setup>
const props = defineProps({
  /**
   * 表單欄位狀態物件，應包含 invalid 和 error?.message
   * @type {{ invalid?: boolean, error?: { message?: string } }}
   */
  field: {
    type: Object,
    default: () => ({}),
  },
});
</script>

<template>
  <Message v-if="field?.invalid" severity="error" size="small" variant="simple">
    {{ field?.error?.message }}
  </Message>
</template>
