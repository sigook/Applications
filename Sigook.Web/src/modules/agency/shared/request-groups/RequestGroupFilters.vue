<template>
  <b-field grouped group-multiline class="request-group-filters">
    <b-field class="input-no-arrows request-group-filter-number">
      <b-input :model-value="filter.numberId" type="number" placeholder="Request #" icon="pound" size="is-small"
        @update:modelValue="onNumberIdChanged" @keypress="onInputEntered"></b-input>
    </b-field>
    <b-field>
      <b-input :model-value="filter.name" placeholder="Search name or email" icon="magnify" size="is-small"
        @update:modelValue="(value: string) => patch({ name: value })" @keypress="onInputEntered"></b-input>
    </b-field>
    <b-field>
      <b-autocomplete v-model="companyTerm" :data="companies" field="value" placeholder="Client" icon="domain"
        size="is-small" open-on-focus clearable append-to-body @typing="searchCompanies" @select="onCompanySelected">
      </b-autocomplete>
    </b-field>
    <b-field>
      <b-input :model-value="filter.jobTitle" placeholder="Position" icon="briefcase-outline" size="is-small"
        @update:modelValue="(value: string) => patch({ jobTitle: value })" @keypress="onInputEntered"></b-input>
    </b-field>
    <slot></slot>
    <b-field>
      <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statusOptions" open-on-focus
        field="value" icon="label" placeholder="Status" append-to-body></b-taginput>
    </b-field>
    <b-field>
      <b-datepicker size="is-small" :mobile-native="false" placeholder="Start date" range v-model="startAtSelected"
        :icon-right="startAtSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable append-to-body
        @icon-right-click="startAtSelected = []"></b-datepicker>
    </b-field>
  </b-field>
</template>

<script setup lang="ts" generic="F extends RequestGroupFilter">
import { computed, ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import { getAgencyCompaniesList } from '@/modules/agency/recruiting/clients/api';
import type { CatalogItem } from '@/shared/types/common';
import type { RequestGroupFilter } from '@/modules/agency/shared/request-groups/types';

const props = defineProps<{ statusOptions: CatalogItem<number>[] }>();
const filter = defineModel<F>('filter', { required: true });
const emit = defineEmits<{ (e: 'search'): void }>();

const companies = ref<CatalogItem[]>([]);
const companyTerm = ref('');

function patch(changes: Partial<RequestGroupFilter>) {
  filter.value = { ...filter.value, ...changes };
}

function search(changes: Partial<RequestGroupFilter>) {
  patch({ ...changes, pageIndex: 1 });
  emit('search');
}

const statusesSelected = computed<CatalogItem<number>[]>({
  get: () => props.statusOptions.filter((option) => filter.value.statuses?.includes(option.id)),
  set: (options) => search({ statuses: options.length ? options.map((option) => option.id) : undefined }),
});

const startAtSelected = computed<Date[]>({
  get: () => filter.value.startAtFrom && filter.value.startAtTo
    ? [new Date(filter.value.startAtFrom), new Date(filter.value.startAtTo)]
    : [],
  set: (dates) => search({
    startAtFrom: dates[0]?.toISOString() ?? null,
    startAtTo: dates[1]?.toISOString() ?? null,
  }),
});

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') search({});
}

function onNumberIdChanged(value: string | number) {
  patch({ numberId: value === '' || value === null ? null : Number(value) });
}

function searchCompanies(term: string) {
  if (term.length < 3) {
    companies.value = [];
    return;
  }
  getAgencyCompaniesList(term)
    .then((response) => {
      companies.value = response;
    })
    .catch((error) => showAlertError(error));
}

function onCompanySelected(company: CatalogItem | null) {
  search({ companyProfileId: company?.id ?? null });
}
</script>

<style scoped lang="scss">
.request-group-filters {
  margin-bottom: 12px;
}

.request-group-filter-number :deep(.input) {
  width: 110px;
}
</style>
