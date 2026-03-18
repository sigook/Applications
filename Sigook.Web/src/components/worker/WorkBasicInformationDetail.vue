<template>
  <section>
    <div class="button-right">
      <h3 class="section-title mt-0">{{ $t("BasicInformation") }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalBasicInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ $t('WorkerFullName') }}</span>
        <span>
          <p class="fw-200 margin-0">
            {{ worker.firstName }} {{ worker.middleName }} {{ worker.lastName }} {{ worker.secondLastName }}
          </p>
        </span>
      </div>
      <div>
        <span>{{ $t('WorkerBirthday') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.birthDay | dateMonth }}</p>
        </span>
      </div>
      <div>
        <span>{{ $t('WorkerGender') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.gender ? worker.gender.value : null }}</p>
        </span>
      </div>
      <div>
        <span>{{ $t('WorkerHasVehicle') }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.hasVehicle ? $t('Yes') : $t('No') }}</p>
        </span>
      </div>
    </div>
    <!-- custom modal -->
    <b-modal v-model="modalBasicInformation" @close="modalBasicInformation = false" width="500px">
      <basic-information-edit :data="worker" @closeModal="() => closeModalEdit()" />
    </b-modal>

    <!-- end custom modal -->
  </section>
</template>

<script lang="ts">
export default {
  props: ['worker'],
  data() {
    return {
      modalBasicInformation: false
    }
  },
  methods: {
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalBasicInformation = false
    }
  },
  components: {
    basicInformationEdit: () => import("./WorkBasicInformationForm.vue")
  }
}
</script>