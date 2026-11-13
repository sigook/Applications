<template>
  <HoverTooltip v-if="name && color !== null" :label="name" class="agency-badge">
    <span class="request-flag" :class="`request-flag--agency-${color}`" :aria-label="`Agency: ${name}`">
      {{ agencyCode(name) }}
    </span>
  </HoverTooltip>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import HoverTooltip from '@/shared/ui/HoverTooltip.vue';
import { agencyCode, useAgencyName } from '@/modules/agency/shared/useAgencyName';

const { agencyId } = defineProps<{
  agencyId?: string | null;
}>();

const { agencyName, agencyColor } = useAgencyName();

const name = computed(() => agencyName(agencyId));
const color = computed(() => agencyColor(agencyId));
</script>

<style lang="scss" scoped>
.agency-badge {
  vertical-align: middle;
  line-height: 1;
}
</style>
