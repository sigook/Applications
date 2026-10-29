<template>
  <b-button tag="router-link" :to="{ path: `/worker-requests/applied/${requestId}`, query: { tab: 'punch' } }"
    type="is-danger" icon-left="clock-outline" icon-right="chevron-right" :expanded="expanded" :size="size"
    @click.stop>
    OPEN PUNCH CARD
  </b-button>
</template>

<script setup lang="ts">
defineProps<{
  requestId: string;
  expanded?: boolean;
  size?: string;
}>();
</script>
