<template>
  <b-modal has-modal-card v-model="complianceOpen" width="820px" :destroy-on-hide="true">
    <applicant-compliance-modal v-if="target" :request-id="target.requestId" :applicant-id="target.applicantId"
      :name="target.name" :status="target.status" :worker-profile-id="target.workerProfileId"
      @updated="emit('updated')" @close="complianceOpen = false" />
  </b-modal>
</template>

<script setup lang="ts">
import ApplicantComplianceModal from '@/components/agency_request/ApplicantComplianceModal.vue';
import type { ApplicantActionTarget } from '@/composables/useApplicantActions';

defineProps<{ target: ApplicantActionTarget | null }>();
const emit = defineEmits<{ (e: 'updated'): void }>();

const complianceOpen = defineModel<boolean>('complianceOpen', { required: true });
</script>
