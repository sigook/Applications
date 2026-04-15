<template>
  <section>
    <div class="button-right">
      <h3 class="section-title">{{ "SIN/SSN" }}</h3>
      <b-button type="is-info" outlined rounded icon-right="pencil"
        @click="modalSocialInsurance = true"></b-button>
    </div>
    <div class="worker-documents">

      <div v-if="worker.socialInsurance">
        <span>{{ "SIN/SSN#" }}</span>
        <span>
          <p class="fw-200 margin-0">
            <b-button size="is-small" type="is-ghost" :icon-right="showSin ? 'eye-off' : 'eye'" @click="showSin = !showSin"></b-button>
            <i v-if="showSin">{{ worker.socialInsurance }}</i>
            <i v-else>{{ sin(worker.socialInsurance) }}</i> |
            <i>{{ "Expire" }}: </i>
            <i v-if="worker.socialInsuranceExpire">{{ date(worker.dueDate) }}</i>
          </p>
        </span>
      </div>
      <div v-if="worker.socialInsuranceFile && worker.socialInsuranceFile.fileName">
        <span>{{ "File" }}</span>
        <span class="fw-200 margin-0">
          <a :href="worker.socialInsuranceFile.pathFile" download target="_blank">
            {{ filename(worker.socialInsuranceFile.fileName) }}
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

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { sin, date, filename } from '@/utils/filters';

export default {
  props: ['worker'],
  data() {
    return {
      modalSocialInsurance: false,
      showSin: false
    }
  },
  methods: {
    sin,
    date,
    filename,
    closeModalEdit() {
      this.$emit('updateProfile', true);
      this.modalSocialInsurance = false;
    }
  },
  components: {
    socialInsuranceEdit: defineAsyncComponent(() => import("./WorkSinForm.vue"))
  }
}
</script>