<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Agencies" :count="totalItems" :crumbs="moduleCrumbs" />
    <div>
      <SigookGrid ref="grid" :fetch="loadAgencies" v-model:params="serverParams" :sort-map="sortMap"
        @update:loading="(value) => isLoading = value" @loaded="(total) => totalItems = total">
        <template #actions>
          <b-button tag="router-link" to="/sales/agencies/create" icon-left="plus">Create</b-button>
        </template>
        <b-table-column field="fullName" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <router-link :to="{ path: '/sales/agencies/' + props.row.id }">
              <span class="is-block">{{ props.row.fullName }}</span>
              <template v-for="(location, index) in props.row.locations">
                <p v-if="index < 2" :key="location">
                  <i class="fz-2 block">{{ location }}</i>
                </p>
              </template>
              <p v-if="props.row.locations && props.row.locations.length > 2">
                <i class="fz-2 block">See details...</i>
              </p>
            </router-link>
          </template>
        </b-table-column>
        <b-table-column field="email" label="Email" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.email" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span class="is-block">{{ props.row.email }}</span>
          </template>
        </b-table-column>
        <b-table-column field="agencyType" label="Type" sortable searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="agencyTypesSelected" autocomplete :data="appGlobals.$agencyTypes" open-on-focus
              field="label" icon="label" placeholder="Select Type" @update:modelValue="onAgencyTypeSelected" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <b-tag size="is-medium" rounded>
              {{ agencyType(props.row.agencyType) }}
            </b-tag>
          </template>
        </b-table-column>
      </SigookGrid>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { getAgenciesList } from '@/api/agencyApi';
import type { AgencyListFilter, AgencyTypeOption } from '@/types/agency';
import type { GridHandle } from '@/types/common';
import { agencyType } from '@/utils/filters';
import { appGlobals } from '@/varaibles';
import SigookGrid from '@/components/SigookGrid.vue';
import PageHeader from '@/components/PageHeader.vue';
import { useModuleBase } from '@/composables/useModuleBase';

const { moduleCrumbs } = useModuleBase();
const agencyStore = useAgencyStore();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  fullName: 0,
  email: 1,
  agencyType: 2,
};

const isLoading = ref(true);
const totalItems = ref(0);
const agencyTypesSelected = ref<AgencyTypeOption[]>([]);
const serverParams = ref<AgencyListFilter>(agencyStore.agencyListFilter ?? {
  sortBy: 0,
  isDescending: false,
});

function loadAgencies(params: AgencyListFilter) {
  agencyStore.updateAgencyListFilter(params);
  return getAgenciesList(params);
}

function onAgencyTypeSelected() {
  serverParams.value.agencyTypes = agencyTypesSelected.value.map((t) => t.value);
  grid.value?.search();
}
</script>
