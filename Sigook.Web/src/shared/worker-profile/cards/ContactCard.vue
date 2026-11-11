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
            <dd v-if="worker.location" class="is-capitalized">{{ address }}</dd>
            <dd v-else class="profile-empty">—</dd>
          </div>
          <div class="profile-field">
            <dt class="profile-field-label">Mobile</dt>
            <dd v-if="worker.mobileNumber"><copy-link kind="phone" :value="worker.mobileNumber" /></dd>
            <dd v-else class="profile-empty">—</dd>
          </div>
          <div v-if="worker.phone" class="profile-field">
            <dt class="profile-field-label">Phone</dt>
            <dd><copy-link kind="phone" :value="worker.phone" :ext="worker.phoneExt" /></dd>
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
            <dd v-if="worker.contactEmergencyPhone"><copy-link kind="phone" :value="worker.contactEmergencyPhone" /></dd>
            <dd v-else class="profile-empty">—</dd>
          </div>
          <div class="profile-field">
            <dt class="profile-field-label">Health problems / allergies</dt>
            <dd v-if="worker.haveAnyHealthProblem">
              {{ [worker.healthProblem, worker.otherHealthProblem].filter(Boolean).join(' · ') || 'Yes' }}
            </dd>
            <dd v-else>None</dd>
          </div>
        </dl>
      </div>
    </div>

    <b-modal custom-content-class="card" v-model="isModalOpen" width="800px">
      <contact-information-form v-if="activeForm === 'contact'" :data="worker" @closeModal="onSaved" />
      <emergency-information-form v-else-if="activeForm === 'emergency'" :data="worker" @closeModal="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';
import ContactInformationForm from '@/shared/worker-profile/forms/WorkContactInformationForm.vue';
import EmergencyInformationForm from '@/shared/worker-profile/forms/WorkEmergencyInformationForm.vue';

type ContactForm = 'contact' | 'emergency';

const { worker } = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<(e: 'updateProfile') => void>();

const isModalOpen = ref(false);
const activeForm = ref<ContactForm | null>(null);

const address = computed(() => {
  const location = worker.location;
  if (!location) {
    return '';
  }
  const city = [location.city?.value, location.city?.province?.code].filter(Boolean).join(', ');
  return [location.address, city, location.postalCode].filter(Boolean).join(' ');
});

const emergencyName = computed(() =>
  [worker.contactEmergencyName, worker.contactEmergencyLastName].filter(Boolean).join(' '),
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
