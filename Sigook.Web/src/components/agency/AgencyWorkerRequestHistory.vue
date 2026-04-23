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

        <pagination v-if="data"
                    :total-pages="data.totalPages"
                    :index-page="data.pageIndex"
                    :size-page="size"
                    @changePage="(index) => loadRequestHistory(index)">
        </pagination>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAgencyWorkerProfileRequestHistory } from '@/api/agencyWorkerApi';
import Pagination from "../../components/Paginator.vue";
import ContainerRequest from "../agency/AgencyWorkerRequestHistoryContainer.vue";

const props = defineProps<{ workerId: any }>();

const size = ref(10);
const currentPage = ref(1);
const data = ref<any>(null);
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
