<template>
  <div v-if="jobLocation">
    <span class="has-text-weight-bold mr-2 block mb-0">Location</span>
    <p class="has-text-weight-normal fz-14 p-1">{{ jobLocation.formattedAddress }}
      <br v-if="jobLocation.mainIntersection">
      <i v-if="jobLocation.mainIntersection">{{ jobLocation.mainIntersection }}</i>
      <br><i>{{ jobLocation.entrance }}</i>
    </p>
    <div class="locations-aside-map" v-if="jobLocation">
      <iframe v-if="jobLocation.latitude && jobLocation.longitude"
        :src="'https://www.google.com/maps/embed/v1/place?key=AIzaSyDj0QAxxsRhSUXsZ-pSKlRh62vsK362xqs&q=' + jobLocation.latitude + ',' + jobLocation.longitude + '&zoom=13'"
        allowfullscreen width="100%" height="400px" frameborder="0"
        style="border:0; height: calc(100% + 110px); margin-top: -110px;"></iframe>
    </div>
    <p class="p-1" v-if="jobLocation.latitude && jobLocation.longitude">
      <a :href="'https://www.google.com/maps/search/?api=1&query=' + jobLocation.latitude + ',' + jobLocation.longitude"
        target="_blank" rel="noopener noreferrer">
        Open in Google Maps
      </a>
    </p>
  </div>
</template>
<script setup lang="ts">
defineProps<{ jobLocation?: any }>();
</script>
