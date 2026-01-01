<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerLocationPreferences") }}</span>
      <span class="width-70 items">
        <span class="inline fw-200 bg-gray" :key="'locationpref' + item.value"
          v-for="item in worker.locationPreferences"> {{ item.value }} </span>
      </span>
      <button class="actions btn-icon-sm btn-icon-edit button-top-m8" type="button"
        @click="modalLocation = true">Edit</button>
    </div>
    <b-modal v-model="modalLocation" width="500px" max-height="80vh">
      <location-preferences-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalLocation: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalLocation = false
    }
  },
  components: {
    locationPreferencesEdit: () => import("./WorkLocationPreferencesForm")
  }
}
</script>