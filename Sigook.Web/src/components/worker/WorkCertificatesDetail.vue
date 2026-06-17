<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>

    <div class="d-flex align-items-center justify-content-between">
      <h3 class="fw-bold fz-0">{{ 'Certificates' }}</h3>
      <b-button type="is-primary" icon-right="plus" @click="modalCertificate = true">
        Add Certificate
      </b-button>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in localWorker.certificates"
        v-bind:key="'certificates' + index">

        <div class="d-flex align-items-center justify-content-between">
          <a :href="item.pathFile" target="_blank" download>
            <h4 class="fw-normal">{{ filename(item.fileName) }} <span class="download-button"></span></h4>
          </a>
          <div class="actions text-end">
            <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
              <button class="btn-icon-sm btn-icon-delete bg-transparent" type="button" @click="confirmDelete(item)">
                {{ "Delete" }}
              </button>
            </b-tooltip>
          </div>
        </div>
        <div class="fz-1">
          <p>
            <strong class="fw-normal">{{ item.description }}</strong>
          </p>
        </div>
      </div>
    </div>
    <!-- custom CREATE modal -->
    <transition name="modal">
      <div v-if="modalCertificate" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light overflow-initial">
              <span class="fz1 fw-bold">{{ "Certificates" }}</span>
              <button @click="modalCertificate = false" type="button" class="cross-icon">
                {{ 'Close' }}
              </button>
              <certificate-edit :data="localWorker" @closeModal="() => closeModalEdit()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
  </section>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { showAlertConfirm, showAlertError } from '@/utils/toast';
import { filename } from '@/utils/filters';
import { deleteWorkerCertificates } from '@/api/workerApi';
import CertificateEdit from './WorkCertificatesForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{
  (e: 'updateProfile', value: boolean): void;
  (e: 'update:worker', value: any): void;
}>();

const modalCertificate = ref(false);
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
  modalCertificate.value = false;
}

function confirmDelete(certificate: any) {
  showAlertConfirm('Are you sure?', 'You want to delete this document')
    .then((response) => {
      if (response) {
        isLoading.value = true;
        deleteWorkerCertificates(localWorker.value.id, certificate.id)
          .then(() => {
            isLoading.value = false;
            localWorker.value.certificates = localWorker.value.certificates.filter((d: any) => d.id !== certificate.id);
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
