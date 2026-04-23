<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "SIN/SSN" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalSocialInsurance = true"></b-button>
    </div>
    <div class="worker-documents">

      <div v-if="props.worker.socialInsurance">
        <span>{{ "SIN/SSN#" }}</span>
        <span>
          <p class="fw-200 margin-0">
            <b-button size="is-small" type="is-ghost" :icon-right="showSin ? 'eye-off' : 'eye'" @click="showSin = !showSin"></b-button>
            <i v-if="showSin">{{ props.worker.socialInsurance }}</i>
            <i v-else>{{ sin(props.worker.socialInsurance) }}</i> |
            <i>{{ "Expire" }}: </i>
            <i v-if="props.worker.socialInsuranceExpire">{{ date(props.worker.dueDate) }}</i>
          </p>
        </span>
      </div>
      <div v-if="props.worker.socialInsuranceFile && props.worker.socialInsuranceFile.fileName">
        <span>{{ "File" }}</span>
        <span class="fw-200 margin-0">
          <a :href="props.worker.socialInsuranceFile.pathFile" download target="_blank">
            {{ filename(props.worker.socialInsuranceFile.fileName) }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
    </div>
    <b-modal v-model="modalSocialInsurance" width="500px">
      <social-insurance-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { sin, date, filename } from '@/utils/filters';
import SocialInsuranceEdit from './WorkSinForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modalSocialInsurance = ref(false);
const showSin = ref(false);

function closeModalEdit() {
  emit('updateProfile', true);
  modalSocialInsurance.value = false;
}
</script>
