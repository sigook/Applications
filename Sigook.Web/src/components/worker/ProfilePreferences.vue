<template>
  <div>
    <div class="profile-worker">
      <section id="lift">
        <lift :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="availability" :class="{ 'missing': props.worker.availabilities.length === 0 }">
        <availability :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="availabletime" :class="{ 'missing': props.worker.availabilityTimes.length === 0 }">
        <availability-times :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>

      <section id="availabledays" :class="{ 'missing': props.worker.availabilityDays.length === 0 }">
        <availability-days :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>

      <section id="locationpreferences" :class="{ 'missing': props.worker.locationPreferences.length === 0 }">
        <location-preferences :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>
      <emergency-information id="emergencyinformation" :class="{ 'missing': !props.worker.contactEmergencyPhone }"
        :worker="props.worker" @updateProfile="() => updateProfile()" />
      <section id="skills" :class="{ 'missing': props.worker.skills.length === 0 }">
        <skills :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="languages">
        <languages :worker="props.worker" @updateProfile="() => updateProfile()" />
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import Skills from '../../components/worker/WorkSkillsDetail.vue';
import Languages from '../../components/worker/WorkLanguagesDetail.vue';
import Lift from '../../components/worker/WorkLiftDetail.vue';
import Availability from '../../components/worker/WorkAvailabilitiesDetail.vue';
import AvailabilityTimes from '../../components/worker/WorkAvailabilityTimesDetail.vue';
import AvailabilityDays from '../../components/worker/WorkAvailabilityDaysDetail.vue';
import LocationPreferences from '../../components/worker/WorkLocationPreferencesDetail.vue';
import EmergencyInformation from './WorkEmergencyInformationDetail.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

function updateProfile() {
  emit('updateProfile');
}
</script>
