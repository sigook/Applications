<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ $t("SocialInsurance") }}</h3>
      <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="modalSocialInsurance = true">Edit</button>
    </div>
    <div class="worker-documents">

      <div v-if="worker.socialInsurance">
        <span>{{ $t("WorkerSocialInsuranceNumber") }}</span>
        <span>
          <p class="fw-200 margin-0">
            <button @click="showSin = !showSin" class="button-view"></button>
            <i v-if="showSin">{{ worker.socialInsurance }}</i>
            <i v-else>{{ worker.socialInsurance | sin }}</i> |
            <i>{{ $t("WorkerDueDate") }}: </i>
            <i v-if="worker.socialInsuranceExpire">{{ worker.dueDate | date }}</i>
          </p>
        </span>
      </div>
      <div v-if="worker.socialInsuranceFile && worker.socialInsuranceFile.fileName">
        <span>{{ $t("File") }}</span>
        <span class="fw-200 margin-0">
          <a :href="worker.socialInsuranceFile.pathFile" download target="_blank">
            {{ worker.socialInsuranceFile.fileName | filename }}
            <span class="download-button"></span>
          </a>
        </span>
      </div>
    </div>
    <b-modal v-model="modalSocialInsurance" width="500px">
      <social-insurance-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalSocialInsurance: false,
      showSin: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalSocialInsurance = false;
    }
  },
  components: {
    socialInsuranceEdit: () => import("./WorkSinForm")
  }
}
</script>