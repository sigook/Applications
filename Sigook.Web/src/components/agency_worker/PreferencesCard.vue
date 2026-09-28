<template>
  <profile-card title="Work preferences">
    <template #actions>
      <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
        <template #trigger>
          <button type="button" class="profile-link">Edit <b-icon icon="menu-down" size="is-small" /></button>
        </template>
        <b-dropdown-item v-for="field in fields" :key="field.form" aria-role="listitem" @click="open(field.form)">
          {{ field.label }}
        </b-dropdown-item>
      </b-dropdown>
    </template>

    <dl class="profile-fields is-two">
      <div v-for="field in fields" :key="field.form" class="profile-field">
        <dt class="profile-field-label">{{ field.label }}</dt>
        <dd v-if="field.values.length" class="profile-tags">
          <span v-for="value in field.values" :key="value" class="profile-tag">{{ value }}</span>
        </dd>
        <dd v-else class="profile-empty">—</dd>
      </div>
    </dl>

    <b-modal custom-content-class="card" v-model="isModalOpen" :width="activeForm === 'days' ? '520px' : '500px'">
      <availabilities-form v-if="activeForm === 'availability'" :data="props.worker" @closeModal="onSaved" />
      <availability-times-form v-else-if="activeForm === 'times'" :data="props.worker" @closeModal="onSaved" />
      <availability-days-form v-else-if="activeForm === 'days'" :data="props.worker" @closeModal="onSaved" />
      <location-preferences-form v-else-if="activeForm === 'locations'" :data="props.worker" @closeModal="onSaved" />
      <lift-form v-else-if="activeForm === 'lift'" :data="props.worker" @closeModal="onSaved" />
      <languages-form v-else-if="activeForm === 'languages'" :data="props.worker" @closeModal="onSaved" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { WorkerProfileDetail } from '@/types/worker';
import ProfileCard from './ProfileCard.vue';
import AvailabilitiesForm from '@/components/worker/WorkAvailabilitiesForm.vue';
import AvailabilityTimesForm from '@/components/worker/WorkAvailabilityTimesForm.vue';
import AvailabilityDaysForm from '@/components/worker/WorkAvailabilityDaysForm.vue';
import LocationPreferencesForm from '@/components/worker/WorkLocationPreferencesForm.vue';
import LiftForm from '@/components/worker/WorkLiftForm.vue';
import LanguagesForm from '@/components/worker/WorkLanguagesForm.vue';

type PreferenceForm = 'availability' | 'times' | 'days' | 'locations' | 'lift' | 'languages';

interface PreferenceField {
  form: PreferenceForm;
  label: string;
  values: string[];
}

const props = defineProps<{ worker: WorkerProfileDetail }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const isModalOpen = ref(false);
const activeForm = ref<PreferenceForm | null>(null);

const fields = computed<PreferenceField[]>(() => [
  { form: 'availability', label: 'Availability', values: props.worker.availabilities.map((i) => i.value) },
  { form: 'times', label: 'Shifts', values: props.worker.availabilityTimes.map((i) => i.value) },
  { form: 'days', label: 'Days', values: props.worker.availabilityDays.map((i) => i.value) },
  { form: 'locations', label: 'Preferred locations', values: props.worker.locationPreferences.map((i) => i.value) },
  { form: 'lift', label: 'Can lift up to', values: props.worker.lift ? [props.worker.lift.value] : [] },
  { form: 'languages', label: 'Languages', values: props.worker.languages.map((i) => i.value) },
]);

function open(form: PreferenceForm) {
  activeForm.value = form;
  isModalOpen.value = true;
}

function onSaved() {
  isModalOpen.value = false;
  emit('updateProfile');
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/agency-worker-profile';
</style>
