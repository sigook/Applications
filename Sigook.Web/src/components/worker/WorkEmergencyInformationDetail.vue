<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "Emergency information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil" @click="modalEmergencyInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ 'Do you have any health problems / allergies?' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.haveAnyHealthProblem ? "Yes" : "No" }}</p>
        </span>
      </div>
      <div v-if="worker.haveAnyHealthProblem">
        <span>{{ 'Which' }} ?</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.healthProblem }}</p>
        </span>
      </div>
      <div v-if="worker.haveAnyHealthProblem">
        <span>{{ 'Other allergies' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.otherHealthProblem }}</p>
        </span>
      </div>

      <p class="fw-400 fz-0 is-italic margin-bottom-10">{{ 'In case of emergency notify' }}: </p>
      <div>
        <span>{{ 'Name' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.contactEmergencyName }} {{ worker.contactEmergencyLastName }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Phone' }}</span>
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

<script lang="ts">
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
    emergencyInformationEdit: () => import("./WorkEmergencyInformationForm.vue")
  }
}
</script>