<template>
    <div>
        <b-loading v-model="isLoading"></b-loading>
        <div v-if="data">
            <container-request
                    v-for="request in data.items"
                    v-bind:key="request.id"
                    :data="request">
            </container-request>
        </div>

        <b-pagination v-if="data && data.totalItems > size"
                      v-model="currentPage"
                      :total="data.totalItems"
                      :per-page="size"
                      size="is-small"
                      rounded
                      @change="loadRequestHistory">
        </b-pagination>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getAgencyWorkerProfileRequestHistory } from '@/modules/agency/recruiting/workers/api';
import ContainerRequest from "@/modules/agency/recruiting/workers/components/AgencyWorkerRequestHistoryContainer.vue";
import type { AgencyWorkerRequestHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { PaginatedList } from '@/shared/types/common';

const props = defineProps<{ workerId: string }>();

const size = ref(10);
const currentPage = ref(1);
const data = ref<PaginatedList<AgencyWorkerRequestHistoryItem> | null>(null);
const isLoading = ref(false);

function loadRequestHistory(page: number) {
  isLoading.value = true;
  getAgencyWorkerProfileRequestHistory(props.workerId, { size: size.value, page })
    .then(response => {
      data.value = response;
      isLoading.value = false;
    })
    .catch(error => {
      showAlertError(error);
      isLoading.value = false;
    });
}

loadRequestHistory(currentPage.value);
</script>
