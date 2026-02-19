<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerLocationPreferences") }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in worker.locationPreferences" :key="'locationpref' + item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLocation = true"></b-button>
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