<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ "Availability" }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in worker.availabilities" :key="item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalAvailability = true"></b-button>
    </div>
     <b-modal v-model="modalAvailability" width="500px">
      <availability-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script lang="ts">
export default {
  props: ['worker'],
  data() {
    return {
      modalAvailability: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalAvailability = false
    }
  },
  components: {
    availabilityEdit: () => import("./WorkAvailabilitiesForm.vue")
  }
}
</script>