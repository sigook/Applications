<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <SigookGrid ref="grid" :fetch="getAgencyWorkers" v-model:params="serverParams" :sort-map="sortMap"
        @update:loading="(value) => isLoading = value" @cellclick="onCellClick">
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
          <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
        </b-table-column>
        <b-table-column field="numberId" label="ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
          </template>
        </b-table-column>
        <b-table-column field="fullName" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.fullName" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <span class="is-block">
              {{ props.row.fullName }}
              <b-icon v-if="props.row.approvedToWork" icon="check-all" size="is-small"></b-icon>
              <b-icon v-if="props.row.dnu" icon="alert" size="is-small" type="is-danger"></b-icon>
            </span>
            <p>
              <i class="fz-2 is-lowercase block">
                <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
              </i>
            </p>
          </template>
        </b-table-column>
        <b-table-column field="mobileNumber" label="Phone" searchable>
          <template v-slot:searchable>
            <b-input :model-value="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
              @update:modelValue="(v) => serverParams.phone = formatPhone(v)"></b-input>
          </template>
          <template v-slot="props">{{ props.row.mobileNumber }}</template>
        </b-table-column>
        <b-table-column field="requestsNumberId" label="Request ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.requestId" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <div v-if="props.row.requests && props.row.requests.length > 0">
              <b-taglist>
                <b-tag v-for="request in props.row.requests" :key="request.id" rounded>
                  {{ request.value }}
                </b-tag>
              </b-taglist>
            </div>
          </template>
        </b-table-column>
        <b-table-column field="createdAt" label="Created At" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onCreatedAtCleared" range v-model="createdAtDatesSelected"
              @update:modelValue="onCreatedAtSelected" append-to-body>
            </b-datepicker>
          </template>
          <template v-slot="props">{{ dateMonth(props.row.createdAt) }}</template>
        </b-table-column>
        <b-table-column field="skills" label="Skills" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.skills" placeholder="Search..." icon="magnify" size="is-small"></b-input>
          </template>
          <template v-slot="props">
            <div v-if="props.row.skills.length > 0">
              <span v-for="(skill, index) in props.row.skills" :key="`${skill}_${index}`"
                class="tag-sm-gray mb-1 mr-1 ellipsis-full">
                {{ skill }}
              </span>
            </div>
            <span v-else class="op3 is-inline-block valign-middle pr-0">Skill</span>
          </template>
        </b-table-column>
        <b-table-column field="isCurrentlyWorking" label="Details" searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="featuresSelected" autocomplete :data="features" open-on-focus
              field="value" icon="label" placeholder="Select Details" @update:modelValue="onFeatureChange" append-to-body>
            </b-taginput>
          </template>
          <template v-slot="props">
            <b-taglist>
              <b-tag v-if="props.row.isCurrentlyWorking" type="is-primary" rounded>Working</b-tag>
              <b-tag v-if="props.row.dnu" type="is-danger" rounded>DNU</b-tag>
              <b-tag v-if="props.row.approvedToWork" type="is-success" rounded>Approved To Work</b-tag>
              <b-tag v-if="props.row.isSubcontractor" type="is-info is-light" rounded>Subcontractor</b-tag>
            </b-taglist>
          </template>
        </b-table-column>
      </SigookGrid>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { workerFeatures as features, type WorkerFeature } from '@/modules/agency/recruiting/workers/workerFeatures';
import { formatPhone } from '@/shared/utils/phoneFormat';
import { dateMonth } from "@/shared/format";
import { getAgencyWorkers } from '@/modules/agency/recruiting/workers/api';
import type { AgencyWorkerFilter, AgencyWorkerListItem } from '@/modules/agency/recruiting/workers/types';
import type { GridHandle } from '@/shared/types/common';
import SigookGrid from '@/shared/ui/SigookGrid.vue';

const props = defineProps<{ company: { id: string } }>();
const router = useRouter();
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  fullName: 0,
  numberId: 1,
  requestsNumberId: 2,
  createdAt: 3,
  skills: 4,
};

const isLoading = ref(false);
const createdAtDatesSelected = ref<Date[]>([]);
const featuresSelected = ref<WorkerFeature[]>([]);
const serverParams = ref<AgencyWorkerFilter>({
  sortBy: 0,
  isDescending: false,
  companyProfileId: props.company.id,
});

function onCellClick(row: AgencyWorkerListItem) {
  router.push(`/recruiting/workers/${row.id}`);
}

function onCreatedAtSelected() {
  serverParams.value.createdAtFrom = createdAtDatesSelected.value[0]?.toISOString() ?? null;
  serverParams.value.createdAtTo = createdAtDatesSelected.value[1]?.toISOString() ?? null;
  grid.value?.search();
}

function onCreatedAtCleared() {
  createdAtDatesSelected.value = [];
  onCreatedAtSelected();
}

function onFeatureChange() {
  serverParams.value.features = featuresSelected.value.map(fs => fs.id);
  grid.value?.search();
}
</script>
