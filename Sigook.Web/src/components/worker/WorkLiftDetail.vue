<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("WorkerYouCanLift") }}</span>
      <span class="width-70">
        <b-tag v-if="worker.lift" type="is-info is-light" size="is-medium" rounded>{{ worker.lift.value }}</b-tag>
      </span>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalLift = true"></b-button>
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