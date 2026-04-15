<template>
  <div>
    <div class="profile-worker">
      <section id="lift">
        <lift :worker="worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="availability" :class="{ 'missing': worker.availabilities.length === 0 }">
        <availability :worker="worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="availabletime" :class="{ 'missing': worker.availabilityTimes.length === 0 }">
        <availability-times :worker="worker" @updateProfile="() => updateProfile()" />
      </section>

      <section id="availabledays" :class="{ 'missing': worker.availabilityDays.length === 0 }">
        <availability-days :worker="worker" @updateProfile="() => updateProfile()" />
      </section>

      <section id="locationpreferences" :class="{ 'missing': worker.locationPreferences.length === 0 }">
        <location-preferences :worker="worker" @updateProfile="() => updateProfile()" />
      </section>
      <emergency-information id="emergencyinformation" :class="{ 'missing': !worker.contactEmergencyPhone }"
        :worker="worker" @updateProfile="() => updateProfile()" />
      <section id="skills" :class="{ 'missing': worker.skills.length === 0 }">
        <skills :worker="worker" @updateProfile="() => updateProfile()" />
      </section>
      <section id="languages">
        <languages :worker="worker" @updateProfile="() => updateProfile()" />
      </section>
    </div>
  </div>
</template>

<script lang="ts">

import { defineAsyncComponent } from 'vue';
export default {
  props: ['worker'],
  components: {
    skills: defineAsyncComponent(() => import("../../components/worker/WorkSkillsDetail.vue")),
    languages: defineAsyncComponent(() => import("../../components/worker/WorkLanguagesDetail.vue")),
    lift: defineAsyncComponent(() => import("../../components/worker/WorkLiftDetail.vue")),
    availability: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilitiesDetail.vue")),
    availabilityTimes: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilityTimesDetail.vue")),
    availabilityDays: defineAsyncComponent(() => import("../../components/worker/WorkAvailabilityDaysDetail.vue")),
    locationPreferences: defineAsyncComponent(() => import("../../components/worker/WorkLocationPreferencesDetail.vue")),
    emergencyInformation: defineAsyncComponent(() => import("./WorkEmergencyInformationDetail.vue"))
  },
  methods: {
    updateProfile() {
      this.$emit('updateProfile');
    }
  }
}
</script>