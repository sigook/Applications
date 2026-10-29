<template>
  <div>
    <b-loading v-model="isLoading" />
    <b-message v-if="!canEdit" type="is-warning" size="is-small" has-icon>
      This request does not use runners. The list is read-only.
    </b-message>

    <RunnersGrid ref="grid" :fetch="loadRunners" v-model:params="serverParams" :sort-map="sortMap" :can-edit="canEdit"
      @update:loading="(value) => isLoading = value" @updated="emit('changed')">
      <template v-if="canEdit" #actions>
        <b-button icon-left="plus" @click="showCreate = true">Add Runner</b-button>
      </template>
    </RunnersGrid>

    <b-modal has-modal-card v-model="showCreate" width="640px">
      <create-runner :request-id="requestId" :is-saving="isCreating" @create="onCreate" @close="showCreate = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { getAgencyRunners, createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerSortBy } from '@/modules/agency/recruiting/requests/runners/types';
import type { AgencyRunnerFilter, CreateRunnerModel } from '@/modules/agency/recruiting/requests/runners/types';
import type { GridHandle } from '@/shared/types/common';
import type { AgencyRequestDetail } from '@/modules/agency/recruiting/requests/types';
import CreateRunner from '@/modules/agency/recruiting/requests/runners/CreateRunner.vue';
import RunnersGrid from '@/modules/agency/recruiting/runners/components/RunnersGrid.vue';

const props = defineProps<{ request?: AgencyRequestDetail | null }>();
const emit = defineEmits<{ (e: 'changed'): void }>();

const route = useRoute();
const requestId = route.params.id as string;
const grid = useTemplateRef<GridHandle>('grid');

const sortMap = {
  name: RunnerSortBy.Name,
  status: RunnerSortBy.Status,
  type: RunnerSortBy.Type,
  createdAt: RunnerSortBy.CreatedAt,
};

const canEdit = computed(() => !!props.request?.usesRunners);

const isLoading = ref(false);
const showCreate = ref(false);
const isCreating = ref(false);

const serverParams = ref<AgencyRunnerFilter>({
  requestId,
  isDescending: true,
  sortBy: RunnerSortBy.CreatedAt,
});

function loadRunners(params: AgencyRunnerFilter) {
  return getAgencyRunners(requestId, params);
}

function onCreate(model: CreateRunnerModel) {
  isCreating.value = true;
  createAgencyRunner(requestId, model)
    .then(() => {
      showCreate.value = false;
      grid.value?.reload();
      emit('changed');
    })
    .catch((error) => showAlertError(error))
    .finally(() => {
      isCreating.value = false;
    });
}
</script>
