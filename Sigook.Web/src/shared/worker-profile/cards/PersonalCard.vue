<template>
  <profile-card title="Personal">
    <template #actions>
      <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
        <template #trigger>
          <b-button type="is-ghost" size="is-small" class="profile-link" icon-right="menu-down">Edit</b-button>
        </template>
        <b-dropdown-item aria-role="listitem" @click="open('basic')">Basic information</b-dropdown-item>
        <b-dropdown-item v-if="props.showLoginEmail" aria-role="listitem" @click="open('email')">Login email</b-dropdown-item>
        <b-dropdown-item aria-role="listitem" @click="open('sin')">SIN/SSN</b-dropdown-item>
      </b-dropdown>
    </template>

    <dl class="profile-fields">
      <div class="profile-field">
        <dt class="profile-field-label">Full name</dt>
        <dd>{{ fullName }}</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">Date of birth</dt>
        <dd>{{ dateMonth(props.worker.birthDay) }}</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">Gender</dt>
        <dd :class="{ 'profile-empty': !props.worker.gender }">{{ props.worker.gender?.value ?? '—' }}</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">Own vehicle</dt>
        <dd>{{ props.worker.hasVehicle ? 'Yes' : 'No' }}</dd>
      </div>
      <div v-if="props.showLoginEmail" class="profile-field">
        <dt class="profile-field-label">Login email</dt>
        <dd>{{ props.worker.email }}</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">SIN/SSN</dt>
        <dd v-if="props.worker.socialInsurance" class="profile-sin">
          {{ showSin ? props.worker.socialInsurance : sin(props.worker.socialInsurance) }}
          <b-button type="is-ghost" size="is-small" class="profile-icon-button" :icon-left="showSin ? 'eye-off' : 'eye'"
            :aria-label="showSin ? 'Hide SIN/SSN' : 'Show SIN/SSN'" @click="showSin = !showSin" />
        </dd>
        <dd v-else class="profile-empty">—</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">SIN/SSN expiry</dt>
        <dd v-if="props.worker.socialInsuranceExpire && props.worker.dueDate">
          {{ dateMonth(props.worker.dueDate) }}
          <span class="profile-status" :class="`is-${sinStatus.tone}`">{{ sinStatus.label }}</span>
        </dd>
        <dd v-else>Does not expire</dd>
      </div>
      <div class="profile-field">
        <dt class="profile-field-label">SIN/SSN file</dt>
        <dd v-if="props.worker.socialInsuranceFile?.fileName">
          <a :href="props.worker.socialInsuranceFile.pathFile" target="_blank" download>
            {{ filename(props.worker.socialInsuranceFile.fileName) }}
          </a>
        </dd>
        <dd v-else class="profile-empty">—</dd>
      </div>
    </dl>

    <b-modal custom-content-class="card" v-model="isModalOpen" width="500px">
      <basic-information-form v-if="activeForm === 'basic'" :data="props.worker" @closeModal="onSaved" />
      <email-form v-else-if="activeForm === 'email' && props.updateEmail" :data="props.worker" :save="props.updateEmail" @closeModal="onSaved" />
      <sin-form v-else-if="activeForm === 'sin'" :data="props.worker" @closeModal="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { dateMonth, filename, sin } from '@/shared/format';
import { expiryStatus } from '@/shared/worker-profile/useWorkerProfileStatus';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';
import BasicInformationForm from '@/shared/worker-profile/forms/WorkBasicInformationForm.vue';
import EmailForm from '@/shared/worker-profile/forms/WorkEmailForm.vue';
import SinForm from '@/shared/worker-profile/forms/WorkSinForm.vue';

type PersonalForm = 'basic' | 'email' | 'sin';

const props = withDefaults(defineProps<{
  worker: WorkerProfileDetail;
  showLoginEmail?: boolean;
  updateEmail?: (workerId: string, model: { newEmail: string }) => Promise<unknown>;
}>(), { showLoginEmail: true });
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const showSin = ref(false);
const isModalOpen = ref(false);
const activeForm = ref<PersonalForm | null>(null);

const fullName = computed(() =>
  [props.worker.firstName, props.worker.middleName, props.worker.lastName, props.worker.secondLastName]
    .filter(Boolean)
    .join(' '),
);

const sinStatus = computed(() => expiryStatus(props.worker.dueDate));

function open(form: PersonalForm) {
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

.profile-sin {
  display: flex;
  align-items: center;
  gap: 4px;
}

.profile-sin .button.profile-icon-button {
  width: 28px;
  height: 28px;
}
</style>
