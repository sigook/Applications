<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerAvailableDays") }}</span>
      <span class="width-70 items">
        <b-taglist>
          <b-tag v-for="item in worker.availabilityDays" :key="'days' + item.value" type="is-info is-light" size="is-medium" rounded>{{ item.value }}</b-tag>
        </b-taglist>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalAvailability = true"></b-button>
    </div>
    <b-modal v-model="modalAvailability" width="520px">
      <availability-days-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>

  </div>
</template>
<script>
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
    availabilityDaysEdit: () => import("./WorkAvailabilityDaysForm")
  }
}
</script>