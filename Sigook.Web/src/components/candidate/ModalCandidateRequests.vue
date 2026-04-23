<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="OrderID, Position">
          <b-autocomplete :data="rows" placeholder="OrderID, Position" :loading="isLoadingList"
            :custom-formatter="(option: any) => `${option.numberId} | ${option.jobTitle} | ${option.companyFullName}`"
            @typing="onInputEntered" @select="(option: any) => optionSelected = option" append-to-body>
            <template v-slot="props">
              <small>
                {{ props.option.numberId }} |
                {{ props.option.jobTitle }} |
                {{ props.option.companyFullName }}
              </small>
            </template>
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" @click="saveRequestApplicant">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAllAgencyRequests, postAgencyRequestApplicant } from "@/api/agencyRequestApi";

const props = defineProps<{ candidateId: number | string }>();
const emit = defineEmits<{ (e: 'onSelectRequest'): void }>();

const isLoading = ref(false);
const isLoadingList = ref(false);
const rows = ref<any[]>([]);
const serverParams = reactive<any>({
  statuses: [0, 1, 4],
});
const optionSelected = ref<any>(null);

function onInputEntered(value: string) {
  serverParams.filter = value;
  loadRequests();
}

function loadRequests() {
  isLoadingList.value = true;
  getAllAgencyRequests(serverParams)
    .then((response: any) => {
      isLoadingList.value = false;
      rows.value = response;
    })
    .catch(error => {
      isLoadingList.value = false;
      showAlertError(error);
    });
}

function saveRequestApplicant() {
  isLoading.value = true;
  postAgencyRequestApplicant(optionSelected.value.id, { candidateId: props.candidateId })
    .then(() => {
      isLoading.value = false;
      emit('onSelectRequest');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
