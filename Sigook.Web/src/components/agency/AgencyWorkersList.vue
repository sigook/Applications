<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Worker" :message="'Type at least 3 characters to search'">
          <b-autocomplete v-model="workerSelected" :data="workers" placeholder="Worker" name="worker" append-to-body
            :loading="isLoadingList" @typing="onWorkerInput" @select="selectWorker"
            :custom-formatter="(option) => `${option.fullName} | ${option.approvedToWork ? 'Approved' : 'Not Approved'}`">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="bookWorker" :disabled="!workerProfileId">Book Worker</b-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getAgencyWorkersDropdown } from "@/api/agencyWorkerApi";
import { bookAgencyRequestWorker } from "@/api/agencyRequestApi";
import { getDialog } from '@/utils/buefyProgrammatic';

const emit = defineEmits<{ (e: 'workerBooked'): void }>();
const route = useRoute();

const isLoading = ref(false);
const requestId = ref<any>(route.params.id);
const isLoadingList = ref(false);
const workers = ref<any[]>([]);
const workerSelected = ref<any>(null);
const workerProfileId = ref<any>(null);

function onWorkerInput(text: string) {
  if (text.length >= 3) {
    searchWorkers(text);
  } else {
    workers.value = [];
  }
}

function searchWorkers(text: string) {
  isLoadingList.value = true;
  getAgencyWorkersDropdown({ searchTerm: text })
    .then(response => {
      isLoadingList.value = false;
      workers.value = response;
    })
    .catch(error => {
      isLoadingList.value = false;
      showAlertError(error);
    });
}

function selectWorker(worker: any) {
  if (worker) {
    workerProfileId.value = worker.workerProfileId;
  } else {
    workerProfileId.value = null;
  }
}

async function bookWorker() {
  getDialog().prompt({
    message: "Starting Date",
    inputAttrs: {
      type: 'date',
      placeholder: 'Date',
      required: true
    },
    confirmText: 'Book',
    onConfirm: async (value: any, dialog: any) => {
      isLoading.value = true;
      await bookAgencyRequestWorker(requestId.value, workerProfileId.value, { startWorking: value }).then(() => {
        isLoading.value = false;
        showAlertSuccess('Booked');
        emit('workerBooked');
        dialog.close();
      }).catch(error => {
        isLoading.value = false;
        showAlertError(error);
      });
    }
  });
}
</script>
