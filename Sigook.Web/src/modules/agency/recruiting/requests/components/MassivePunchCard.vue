
<template>
  <div class="mt-1">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="getAgencyRequestsWorkers" v-model:params="serverParams" :sort-map="sortMap"
        :export="{ url: getTimeSheetUrl, fileName: 'Timesheet' }" :fit-viewport="false" detailed show-detail-icon
        @update:loading="(value) => isLoading = value">
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
          <default-image v-else :name="props.row.name" class="img-30"></default-image>
        </b-table-column>
        <b-table-column field="numberId" label="ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
          </template>
        </b-table-column>
        <b-table-column field="externalId" label="External ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.externalId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">{{ props.row.externalId }}</template>
        </b-table-column>
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.name }}
          </template>
        </b-table-column>
        <b-table-column field="totalHoursApproved" label="Approved Hours" v-slot="props">
          {{ hour(props.row.totalHoursApproved) }}
        </b-table-column>
        <b-table-column field="totalHoursWorker" label="Total Hours" v-slot="props">
          {{ hour(props.row.totalHoursWorker) }}
        </b-table-column>
        <b-table-column field="status" label="Status" sortable searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="filteredStatuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @typing="filterStatuses" @update:modelValue="onStatusSelected" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <b-tag rounded :type="props.row.workerRequestStatus === 3 ? 'is-success' : 'is-danger'">{{ props.row.workerRequestStatus === 3 ? 'Booked' : 'Rejected' }}</b-tag>
          </template>
        </b-table-column>
        <template #detail="props">
          <PunchCard :workerProfileId="props.row.workerProfileId" :worker="props.row"
            :requestId="serverParams.requestId" :request="request" />
        </template>
      </SigookGrid>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import { hour } from '@/shared/format';
import { getAgencyRequestsWorkers } from "@/modules/agency/recruiting/requests/api";
import PunchCard from '@/modules/agency/recruiting/requests/components/AgencyPunchCardWorkerContainer.vue';
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { CatalogItem, GridHandle } from '@/shared/types/common';
import type { AgencyRequestDetail, AgencyRequestWorkerFilter } from '@/modules/agency/recruiting/requests/types';

defineProps<{ request: AgencyRequestDetail }>();

const route = useRoute();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  numberId: 0,
  name: 1,
  status: 2,
  externalId: 6,
};

const isLoading = ref(true);
const statuses: CatalogItem<number>[] = [
  { id: 2, value: 'Rejected' },
  { id: 3, value: 'Booked' },
];
const statusesSelected = ref<CatalogItem<number>[]>([]);
const filteredStatuses = ref<CatalogItem<number>[]>(statuses);
const serverParams = ref<AgencyRequestWorkerFilter>({
  sortBy: 2,
  requestId: route.params.id as string,
  isDescending: true
});

function filterStatuses(text: string) {
  filteredStatuses.value = statuses.filter(s =>
    !statusesSelected.value.some(ss => ss.id === s.id) &&
    s.value.toLowerCase().includes(text.toLowerCase())
  );
}

function onStatusSelected() {
  filteredStatuses.value = statuses.filter(s =>
    !statusesSelected.value.some(ss => ss.id === s.id)
  );
  serverParams.value.statuses = statusesSelected.value.map(ss => ss.id);
  grid.value?.search();
}

const getTimeSheetUrl = computed(() => `/api/agency/recruiting/requests/${serverParams.value.requestId}/TimeSheets/File`);
</script>
