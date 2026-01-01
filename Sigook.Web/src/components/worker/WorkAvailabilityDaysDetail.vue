<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerAvailableDays") }}</span>
      <span class="width-70 items">
        <span class="inline fw-200 bg-gray" :key="'days' + item.value" v-for="item in worker.availabilityDays">
          {{ item.value }} </span>
      </span>
      <button class="actions btn-icon-sm btn-icon-edit button-top-m8" type="button"
        @click="modalAvailability = true">Edit</button>
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