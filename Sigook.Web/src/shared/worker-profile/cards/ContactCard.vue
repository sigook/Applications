<template>
  <profile-card title="Contact & emergency">
    <template #actions>
      <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
        <template #trigger>
          <b-button type="is-ghost" size="is-small" class="profile-link" icon-right="menu-down">Edit</b-button>
        </template>
        <b-dropdown-item aria-role="listitem" @click="open('contact')">Contact information</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('emergency')">Emergency information</b-dropdown-item>
      </b-dropdown>
    </template>

    <div class="profile-fields is-two">
      <div class="contact-group">
        <p class="profile-group-title">Worker</p>
        <dl class="contact-fields">
          <div class="profile-field">
            <dt class="profile-field-label">Address</dt>
            <dd v-if="props.worker.location" class="is-capitalized">{{ address }}</dd>
            <dd v-else class="profile-empty">—</dd>
          </div>
          <div class="profile-field">
            <dt class="profile-field-label">Mobile</dt>
            <dd :class="{ 'profile-empty': !props.worker.mobileNumber }">{{ props.worker.mobileNumber || '—' }}</dd>
          </div>
          <div v-if="props.worker.phone" class="profile-field">
            <dt class="profile-field-label">Phone</dt>
            <dd>
              {{ props.worker.phone }}<template v-if="props.worker.phoneExt"> · Ext. {{ props.worker.phoneExt }}</template>
            </dd>
          </div>
        </dl>
      </div>

      <div class="contact-group">
        <p class="profile-group-title">Emergency contact</p>
        <dl class="contact-fields">
          <div class="profile-field">
            <dt class="profile-field-label">Name</dt>
            <dd :class="{ 'profile-empty': !emergencyName }">{{ emergencyName || '—' }}</dd>
          </div>
          <div class="profile-field">
            <dt class="profile-field-label">Phone</dt>
            <dd :class="{ 'profile-empty': !props.worker.contactEmergencyPhone }">{{ props.worker.contactEmergencyPhone || '—' }}</dd>
          </div>
          <div class="profile-field">
            <dt class="profile-field-label">Health problems / allergies</dt>
            <dd v-if="props.worker.haveAnyHealthProblem">
              {{ [props.worker.healthProblem, props.worker.otherHealthProblem].filter(Boolean).join(' · ') || 'Yes' }}
            </dd>
            <dd v-else>None</dd>
          </div>
        </dl>
      </div>
    </div>

    <b-modal custom-content-class="card" v-model="isModalOpen" width="800px">
      <contact-information-form v-if="activeForm === 'contact'" :data="props.worker" @closeModal="onSaved" />
      <emergency-information-form v-else-if="activeForm === 'emergency'" :data="props.worker" @closeModal="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';
import ContactInformationForm from '@/shared/worker-profile/forms/WorkContactInformationForm.vue';
import EmergencyInformationForm from '@/shared/worker-profile/forms/WorkEmergencyInformationForm.vue';

type ContactForm = 'contact' | 'emergency';

const props = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const isModalOpen = ref(false);
const activeForm = ref<ContactForm | null>(null);

const address = computed(() => {
  const location = props.worker.location;
  if (!location) {
    return '';
  }
  const city = [location.city?.value, location.city?.province?.code].filter(Boolean).join(', ');
  return [location.address, city, location.postalCode].filter(Boolean).join(' ');
});

const emergencyName = computed(() =>
  [props.worker.contactEmergencyName, props.worker.contactEmergencyLastName].filter(Boolean).join(' '),
);

function open(form: ContactForm) {
  activeForm.value = form;
  isModalOpen.value = true;
}

function onSaved() {
  isModalOpen.value = false;
  emit('updateProfile');
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/worker-profile';

.contact-fields {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin: 0;
}
</style>
