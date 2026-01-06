<template>
  <div class="hover-transform">
    <div class="detail-worker-profile">
      <span class="width-30">{{ $t("Email") }}</span>
      <span class="width-70">
        <p class="fw-200 margin-0">{{ worker.email }}</p>
      </span>
      <button class="actions btn-icon-sm btn-icon-edit button-top-m8" type="button"
        @click="modalEmail = true">Edit</button>
    </div>
    <b-modal v-model="modalEmail" width="500px">
      <email-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </div>
</template>
<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalEmail: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalEmail = false
    }
  },
  components: {
    emailEdit: () => import("./WorkEmailForm")
  }
}
</script>