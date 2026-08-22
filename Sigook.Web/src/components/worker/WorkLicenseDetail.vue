<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="is-flex is-align-items-center is-justify-content-space-between">
      <h3 class="has-text-weight-bold fz-0">{{ "Licenses" }}</h3>
      <b-button type="is-primary" icon-right="plus" @click="modalLicense = true">
        Add License
      </b-button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in localWorker.licenses"
        v-bind:key="'licences' + index">
        <div class="is-flex is-align-items-center is-justify-content-space-between">
          <a :href="item.license.pathFile" target="_blank" download>
            <h4 class="has-text-weight-normal">
              {{ filename(item.license.fileName) }}
              <span class="download-button"></span>
            </h4>
          </a>
          <div class="actions has-text-right">
            <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
              <button class="btn-icon-sm btn-icon-delete" type="button" @click="confirmDelete(item.license)">
                {{ "Delete" }}
              </button>
            </b-tooltip>
          </div>
        </div>

        <div class="fz-1">
          <p>
            <strong class="has-text-weight-normal">{{ item.license.description }}</strong>
            <strong class="has-text-weight-normal" v-if="item.number">
              # {{ item.number }}</strong>
          </p>
          <span v-if="item.issued">Issued: {{ dateMonth(item.issued) }} | </span>
          <span v-if="item.expires">Expire: {{dateMonth(item.expires) }}</span>
        </div>
      </div>
    </div>
    <b-modal custom-content-class="card" v-model="modalLicense" width="500px">
      <license-edit :data="localWorker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { showAlertConfirm, showAlertError } from '@/utils/toast';
import { filename, dateMonth } from '@/utils/filters';
import { deleteWorkerLicenses } from '@/api/workerApi';
import LicenseEdit from './WorkLicenseForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{
  (e: 'updateProfile', value: boolean): void;
  (e: 'update:worker', value: any): void;
}>();

const modalLicense = ref(false);
const isLoading = ref(false);
const localWorker = ref<any>(JSON.parse(JSON.stringify(props.worker)));

watch(
  () => props.worker,
  (newVal) => {
    localWorker.value = JSON.parse(JSON.stringify(newVal));
  },
  { deep: true }
);

function closeModalEdit() {
  emit('updateProfile', true);
  modalLicense.value = false;
}

function confirmDelete(license: any) {
  showAlertConfirm('Are you sure', 'You want to delete this document')
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteWorkerLicenses(localWorker.value.id, license.id)
          .then(() => {
            isLoading.value = false;
            localWorker.value.licenses = localWorker.value.licenses.filter((d: any) => d.license.id !== license.id);
            emit('update:worker', localWorker.value);
          })
          .catch((error) => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    })
    .catch((error) => {
      showAlertError(error);
    });
}

</script>
