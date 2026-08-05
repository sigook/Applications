<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-5">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Agencies
        <span class="fw-light fz-1">
          ({{ totalItems }})
        </span>
      </h2>
    </div>
    <div>
      <b-field grouped position="is-right">
        <b-button tag="router-link" to="/sales/agencies/create" icon-left="plus">
          {{ 'Create' }}
        </b-button>
      </b-field>
      <b-table sticky-header height="var(--grid-height)" :data="rows" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" :default-sort="defaultSort"
        v-model:current-page="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="fullName" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <router-link :to="{ path: '/sales/agencies/' + props.row.id }">
                <span class="d-block">{{ props.row.fullName }}</span>
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
              <b-input v-model="serverParams.email" placeholder="Search..." icon="magnify" size="is-small"
                @keypress="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">{{ props.row.email }}</span>
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
        </template>
      </b-table>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useAgencyStore } from '@/stores/agency';
import { showAlertError } from '@/utils/toast';
import { getAgenciesList } from '@/api/agencyApi';
import { agencyType } from '@/utils/filters';
import { useGridSort } from '@/composables/useGridSort';
import { appGlobals } from '@/varaibles';

const agencyStore = useAgencyStore();

const isLoading = ref(true);
const totalItems = ref(0);
const rows = ref<any[]>([]);
const agencyTypesSelected = ref<any[]>([]);
const serverParams = ref<any>({
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
});

const { defaultSort, onSortChange } = useGridSort(serverParams, {
  fullName: 0,
  email: 1,
  agencyType: 2,
}, () => getAgencies());

if (agencyStore.agencyListFilter) {
  serverParams.value = agencyStore.agencyListFilter;
}
getAgencies();

function onPageChange(params: number) {
  serverParams.value.pageIndex = params;
  getAgencies();
}

function onInputEntered(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    getAgencies();
  }
}

function onAgencyTypeSelected() {
  serverParams.value.agencyTypes = agencyTypesSelected.value.map((t) => t.value);
  getAgencies();
}

function getAgencies() {
  isLoading.value = true;
  agencyStore.updateAgencyListFilter(serverParams.value);
  getAgenciesList(serverParams.value)
    .then((agencies: any) => {
      rows.value = agencies.items;
      totalItems.value = agencies.totalItems;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
