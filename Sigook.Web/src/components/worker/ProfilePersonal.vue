<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="profile-information">
      <basic-information id="basicinformation" :worker="props.worker" @updateProfile="() => updateProfile()" />
      <contact-information id="contactinformation" :worker="props.worker" @updateProfile="() => updateProfile()" />
      <social-insurance id="socialinsurance" :class="{ 'missing': !props.worker.socialInsurance }" :worker="props.worker"
        @updateProfile="() => updateProfile()" />
      <documents id="documents" :class="{ 'missing': !props.worker.identificationType1File && !props.worker.identificationType2File }"
        :worker="props.worker" @updateProfile="() => updateProfile()" />
      <resume id="resume" :class="{ 'missing': !props.worker.resume }" :worker="props.worker"
        @updateProfile="() => updateProfile()" />
      <licenses v-if="props.worker && props.worker.licenses" id="licenses" :class="{ 'missing': props.worker.licenses.length === 0 }" :worker="props.worker" @update:worker="emit('update:worker', $event)"
        @updateProfile="() => updateProfile()" />
      <certificates v-if="props.worker && props.worker.certificates" id="certificates" :class="{ 'missing': props.worker.certificates.length === 0 }" :worker="props.worker" @update:worker="emit('update:worker', $event)"
        @updateProfile="() => updateProfile()" />
      <other-documents v-if="props.worker && props.worker.otherDocuments" id="otherdocuments" :class="{ 'missing': props.worker.otherDocuments.length === 0 }" :worker="props.worker"
        :justWhmis="true" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import BasicInformation from './WorkBasicInformationDetail.vue';
import SocialInsurance from './WorkSinDetail.vue';
import Documents from './WorkDocumentsDetail.vue';
import ContactInformation from './WorkContactInformationDetail.vue';
import Resume from './WorkResumeDetail.vue';
import Licenses from './WorkLicenseDetail.vue';
import Certificates from './WorkCertificatesDetail.vue';
import OtherDocuments from './WorkerOtherDocumentsDetail.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{
  (e: 'updateProfile'): void;
  (e: 'update:worker', value: any): void;
}>();

const isLoading = ref(false);

function updateProfile() {
  emit('updateProfile');
}
</script>
