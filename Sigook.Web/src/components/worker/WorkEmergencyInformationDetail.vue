<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ $t("WorkerEmergencyInformation") }}</h3>
      <button class="actions btn-icon-sm btn-icon-edit" type="button"
        @click="modalEmergencyInformation = true">Edit</button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ $t('WorkerDoYouHaveAnyHealthProblemsAllergies') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.haveAnyHealthProblem ? "Yes" : "No" }}</p>
        </span>
      </div>
      <div v-if="worker.haveAnyHealthProblem">
        <span>{{ $t('WorkerWhich') }} ?</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.healthProblem }}</p>
        </span>
      </div>
      <div v-if="worker.haveAnyHealthProblem">
        <span>{{ $t('WorkerOtherAllergies') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.otherHealthProblem }}</p>
        </span>
      </div>

      <p class="fw-400 fz-0 is-italic margin-bottom-10">{{ $t('WorkerInCaseOfEmergencyNotify') }}: </p>
      <div>
        <span>{{ $t('Name') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.contactEmergencyName }} {{ worker.contactEmergencyLastName }}</p>
        </span>
      </div>
      <div>
        <span>{{ $t('Phone') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.contactEmergencyPhone }}</p>
        </span>
      </div>
    </div>
    <b-modal v-model="modalEmergencyInformation" width="800px">
      <emergency-information-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>
  </section>
</template>

<script>
export default {
  props: ['worker'],
  data() {
    return {
      modalEmergencyInformation: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalEmergencyInformation = false
    }
  },
  components: {
    emergencyInformationEdit: () => import("./WorkEmergencyInformationForm")
  }
}
</script>