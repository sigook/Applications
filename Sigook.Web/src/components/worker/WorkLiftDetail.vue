<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerYouCanLift") }}</span>
      <span class="width-70">
        <span class="inline fw-200" v-if="worker.lift"> {{ worker.lift.value }} </span>
      </span>
      <button class="actions btn-icon-sm btn-icon-edit button-top-m8" type="button"
        @click="modalLift = true">Edit</button>
    </div>
    <b-modal v-model="modalLift" width="500px">
      <lift-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalLift: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalLift = false
    }
  },
  components: {
    liftEdit: () => import("./WorkLiftForm")
  }
}
</script>