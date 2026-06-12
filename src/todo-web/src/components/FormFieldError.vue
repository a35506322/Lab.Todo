<script setup>
defineProps({
  /**
   * Regle 欄位狀態物件，應包含 $errors
   * @type {{ $errors?: string[] }}
   */
  field: {
    type: Object,
    default: () => ({}),
  },
});
</script>

<template>
  <Message
    v-for="error of field?.$errors"
    :key="error"
    severity="error"
    size="small"
    variant="simple"
  >
    {{ error }}
  </Message>
</template>
