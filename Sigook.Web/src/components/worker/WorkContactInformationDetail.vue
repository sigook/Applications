<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "Contact information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalContactInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div v-if="props.worker.location">
        <span>{{ 'Address' }}</span>
        <span>
          <p class="fw-200 margin-0 capitalize">
            {{ props.worker.location.address }}
            {{ props.worker.location.city.value }}, {{ props.worker.location.city.province.code }}
            {{ props.worker.location.postalCode }}
          </p>
        </span>
      </div>
      <div>
        <span>{{ 'Mobile Number' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ props.worker.mobileNumber }}</p>
        </span>
      </div>
      <div v-if="props.worker.phone">
        <span>{{ 'Phone' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ props.worker.phone }}
            <span v-if="props.worker.phoneExt" class="fw-200">
              <b>{{ 'Ext.' }}:</b>
              {{ props.worker.phoneExt }}
            </span>
          </p>
        </span>
      </div>
    </div>
    <b-modal v-model="modalContactInformation" width="800px">
      <contact-information-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import ContactInformationEdit from './WorkContactInformationForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalContactInformation = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalContactInformation.value = false;
}
</script>
