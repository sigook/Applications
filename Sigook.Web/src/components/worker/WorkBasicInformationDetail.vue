<template>
  <section>
    <div class="button-right">
      <h3 class="section-title mt-0">{{ "Basic Information" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalBasicInformation = true"></b-button>
    </div>
    <div class="worker-documents">
      <div>
        <span>{{ 'Full Name' }}</span>
        <span>
          <p class="fw-200 margin-0">
            {{ worker.firstName }} {{ worker.middleName }} {{ worker.lastName }} {{ worker.secondLastName }}
          </p>
        </span>
      </div>
      <div>
        <span>{{ 'Date of birth' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ dateMonth(worker.birthDay) }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Gender' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.gender ? worker.gender.value : null }}</p>
        </span>
      </div>
      <div>
        <span>{{ 'Do you have your own vehicle?' }}</span>
        <span>
          <p class="fw-200 margin-0">{{ worker.hasVehicle ? 'Yes' : 'No' }}</p>
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
import { defineAsyncComponent } from 'vue';
import { dateMonth } from '@/utils/filters';

export default {
  props: ['worker'],
  data() {
    return {
      modalBasicInformation: false
    }
  },
  methods: {
    dateMonth,
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalBasicInformation = false
    }
  },
  components: {
    basicInformationEdit: defineAsyncComponent(() => import("./WorkBasicInformationForm.vue"))
  }
}
</script>