<template>
  <b-dropdown v-if="agencies.length > 1" class="agency-filter" aria-role="list" :close-on-click="false"
    append-to-body>
    <template #trigger>
      <b-button icon-left="domain" icon-right="chevron-down" :class="{ 'is-filtered': !allSelected }"
        :aria-label="`Agencies: ${label}`">
        {{ label }}
      </b-button>
    </template>
    <b-dropdown-item custom :focusable="false" aria-role="listitem">
      <b-switch :model-value="allSelected" @update:model-value="onToggleAll">
        <strong>All agencies</strong>
      </b-switch>
    </b-dropdown-item>
    <b-dropdown-item separator></b-dropdown-item>
    <b-dropdown-item v-for="agency in agencies" :key="agency.id" custom :focusable="false" aria-role="listitem">
      <b-switch :model-value="selected.includes(agency.id)"
        @update:model-value="(value: boolean) => onToggle(agency.id, value)">
        <AgencyBadge :agency-id="agency.id" class="mr-1" />
        {{ agency.name }}
      </b-switch>
    </b-dropdown-item>
  </b-dropdown>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useAgencyStore } from '@/modules/agency/store';
import AgencyBadge from '@/modules/agency/shared/AgencyBadge.vue';

const model = defineModel<string | undefined>();

const emit = defineEmits<{
  (e: 'change'): void;
}>();

const agencyStore = useAgencyStore();

const agencies = computed(() => agencyStore.agencies);
const allIds = computed(() => agencies.value.map((a) => a.id));
const selected = computed(() =>
  model.value === undefined ? allIds.value : model.value.split(',').filter((id) => allIds.value.includes(id)));
const allSelected = computed(() => selected.value.length === allIds.value.length);

const label = computed(() => {
  if (allSelected.value) return 'All agencies';
  if (selected.value.length === 0) return 'No agencies';
  if (selected.value.length === 1) return agencies.value.find((a) => a.id === selected.value[0])?.name ?? '1 agency';
  return `${selected.value.length} of ${allIds.value.length} agencies`;
});

function apply(ids: string[]) {
  model.value = ids.length === allIds.value.length ? undefined : ids.join(',');
  emit('change');
}

function onToggleAll(value: boolean) {
  apply(value ? allIds.value : []);
}

function onToggle(id: string, value: boolean) {
  apply(value ? allIds.value.filter((a) => a === id || selected.value.includes(a)) : selected.value.filter((a) => a !== id));
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.agency-filter .button.is-filtered {
  border-color: $blue;
  color: $blue-dark;
}
</style>
